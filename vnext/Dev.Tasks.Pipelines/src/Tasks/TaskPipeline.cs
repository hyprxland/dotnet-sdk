using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;
using Hyprx.Dev.Messaging;
using Hyprx.Results;

namespace Hyprx.Dev.Tasks;

public class TaskPipeline : Pipeline<TaskPipelineContext, TaskResult>
{
    public TaskPipeline()
    {
        this.Use(new ExecuteTaskMiddleware());
        this.Use(new ApplyTaskContextMiddleware());
    }

    public override async Task<TaskResult> RunAsync(TaskPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                context.Result.Cancel();
                return context.Result;
            }

            await this.PipeAsync(context, cancellationToken);
            return context.Result;
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            context.Bus.Send(new TaskFailed(context.Data, ex));
            return context.Result;
        }
    }
}

public class TaskPipelineContext : BusContext
{
    public TaskPipelineContext(BusContext context, CodeTask task, CodeTaskData data)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));
        ArgumentNullException.ThrowIfNull(data, nameof(data));

        this.Task = task;
        this.Data = data;
        this.Result = new TaskResult(data.Id);
    }

    public CodeTask Task { get; }

    public CodeTaskData Data { get; }

    public TaskResult Result { get; }

    public RunStatus Status { get; set; } = RunStatus.Pending; // Default status is Pending
}

public class ApplyTaskContextMiddleware : IPipelineMiddleware<TaskPipelineContext>
{

    public async Task NextAsync(TaskPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.Data;
        var task = context.Task;

        var ctx = new TaskContext(context, data);

        try
        {
            data.Name = task.Name;
            data.Id = task.Id;
            data.Description = task.Description;
            data.Needs = task.Needs ?? Array.Empty<string>();
            data.Uses = task.Uses;

            if (!task.Cwd.HasValue)
                await task.Cwd.ResolveAsync(ctx, cancellationToken);

            data.Cwd = task.Cwd.Value.Length > 0 ? task.Cwd.Value : context.Cwd ?? Environment.CurrentDirectory;

            if (!task.Timeout.HasValue)
                await task.Timeout.ResolveAsync(ctx, cancellationToken);

            data.Timeout = task.Timeout.Value;

            if (!task.Force.HasValue)
                await task.Force.ResolveAsync(ctx, cancellationToken);

            data.Force = task.Force.Value;

            if (!task.If.HasValue)
                await task.If.ResolveAsync(ctx, cancellationToken);

            data.If = task.If.Value;

            if (!task.Env.HasValue)
                await task.Env.ResolveAsync(ctx, cancellationToken);

            if (!task.With.HasValue)
                await task.With.ResolveAsync(ctx, cancellationToken);

            data.Env = task.Env.Value;
            data.Inputs = task.With.Value;

            await next();
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new TaskFailed(data, ex));
        }
    }
}

public class ExecuteTaskMiddleware : IPipelineMiddleware<TaskPipelineContext>
{

    public async Task NextAsync(TaskPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.Data;

        var bus = context.GetService<IMessageBus>();

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                context.Result.Cancel();
                await context.Bus.SendAsync(new TaskCancelled(data));
                return;
            }

            if ((context.Status is RunStatus.Failed or RunStatus.Cancelled) && !data.Force)
            {
                context.Result.Skip();
                await context.Bus.SendAsync(new TaskSkipped(data));
                return;
            }

            if (!data.If)
            {
                context.Result.Skip();
                await context.Bus.SendAsync(new TaskSkipped(data));
                return;
            }

            var defaults = context.Services.GetService(typeof(ExecutionDefaults)) as ExecutionDefaults ?? new ExecutionDefaults();
            var timeout = data.Timeout > 0 ? data.Timeout : defaults.Timeout;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            context.Result.Start();
            if (cts.IsCancellationRequested)
            {
                context.Result.Cancel();
                await context.Bus.SendAsync(new TaskCancelled(data));
                return;
            }

            await context.Bus.SendAsync(new TaskStarted(data));

            try
            {
                var ctx = new TaskContext(context, data);
                var task = context.Task;
                Result<Outputs> result;
                if (task is ITaskHandler delegateTaskHandler)
                {
                    result = await delegateTaskHandler.RunAsync(ctx, cts.Token);
                }
                else
                {
                    if (TaskHandlerMap.Global.TryGetValue(task.Id, out var handler))
                    {
                        result = await handler.RunAsync(ctx, cts.Token);
                    }
                    else
                    {
                        var ex = new InvalidOperationException($"No handler found for task '{task.Id}'.");
                        result = new Result<Outputs>(ex);
                    }
                }

                if (result.IsOk)
                {
                    context.Result.Ok(result.Value);
                    await context.Bus.SendAsync(new TaskCompleted(data));
                }
                else
                {
                    context.Result.Fail(result.Error);
                    await context.Bus.SendAsync(new TaskFailed(data, result.Error));
                }
            }
            catch (Exception ex)
            {
                cts.Dispose();

                // Handle any exceptions that occur during task execution
                context.Result.Fail(ex);
                await context.Bus.SendAsync(new TaskFailed(data, ex));
            }
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new TaskFailed(data, ex));
        }
        finally
        {
            await next();
        }
    }
}