using Hyprx.Dev.Execution;
using Hyprx.Dev.Tasks;
using Hyprx.Extras;

namespace Hyprx.Dev.Jobs;

public class ApplyJobContextMiddleware : IPipelineMiddleware<JobPipelineContext>
{
    public async Task NextAsync(JobPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.JobData;
        var job = context.Job;

        try
        {
            foreach (var kvp in context.Env)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!job.Cwd.HasValue)
                await job.Cwd.ResolveAsync(context, cancellationToken);

            var cwd = job.Cwd.Value;
            data.Cwd = !string.IsNullOrEmpty(cwd) ? cwd : Environment.CurrentDirectory;

            if (!job.Timeout.HasValue)
                await job.Timeout.ResolveAsync(context, cancellationToken);

            data.Timeout = job.Timeout.Value;

            if (!job.Force.HasValue)
                await job.Force.ResolveAsync(context, cancellationToken);

            data.Force = job.Force.Value;

            if (!job.If.HasValue)
                await job.If.ResolveAsync(context, cancellationToken);

            data.If = job.If.Value;

            if (!job.Env.HasValue)
                await job.Env.ResolveAsync(context, cancellationToken);

            var env = job.Env.Value;
            foreach (var kvp in env)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!job.With.HasValue)
                await job.With.ResolveAsync((RunContext)context, cancellationToken);

            data.Inputs = job.With.Value;

            await next();
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            context.Bus.Send(new JobFailed(data, ex));
        }
    }
}

public class RunJobMiddleware : IPipelineMiddleware<JobPipelineContext>
{

    public async Task NextAsync(JobPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.JobData;
        var job = context.Job;

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                context.Result.Cancel();
                context.Bus.Send(new JobCancelled(data));
                return;
            }

            if (context.Status == RunStatus.Failed || (context.Status is RunStatus.Cancelled && !data.Force))
            {
                context.Result.Skip();
                context.Bus.Send(new JobSkipped(data));
                return;
            }

            if (data.If == false)
            {
                context.Result.Skip();
                context.Bus.Send(new JobSkipped(data));
                return;
            }

            var timeout = data.Timeout;
            if (timeout < 0)
            {
                var defaults = context.GetService<ExecutionDefaults>();
                if (defaults == null)
                {
                    defaults = new ExecutionDefaults();
                }

                timeout = defaults.Timeout;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout > 0)
            {
                cts.CancelAfter(timeout);
            }

            var token = cts.Token;

            try
            {
                context.Result.Start();
                context.Bus.Send(new JobStarted(data));

                var pipeline = context.GetService<SequentialTasksPipeline>();
                pipeline ??= new SequentialTasksPipeline();

                var tasks = new TaskMap();
                foreach (var kvp in data.Tasks)
                {
                    tasks[kvp.Key] = kvp.Value;
                }

                var targets = tasks.Values.Select(t => t.Id).ToList();
                var ctx = new SequentialTasksPipelineContext(context, targets, tasks);

                var summary = await pipeline.RunAsync(ctx, token);
                if (summary.Exception is not null)
                {
                    context.Result.Fail(summary.Exception);
                    context.Bus.Send(new JobFailed(data, summary.Exception));
                    return;
                }

                var normalized = job.Id.Underscore();

                foreach (var kvp in ctx.Outputs)
                {
                    if (kvp.Key.StartsWith("task.", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Outputs[$"job.{normalized}.{kvp.Key}"] = kvp.Value;
                    }
                }

                context.Result.Ok();
                context.Bus.Send(new JobCompleted(data));
            }
            catch (Exception ex)
            {
                context.Result.Fail(ex);
                context.Bus.Send(new JobFailed(data, ex));
                return;
            }
            finally
            {
                cts.Dispose();
            }

            await next();
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            context.Bus.Send(new JobFailed(data, ex));
        }
    }
}