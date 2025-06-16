using Hyprx.Extras;
using Hyprx.Rex.Collections;
using Hyprx.Rex.Execution;
using Hyprx.Rex.Messaging;
using Hyprx.Secrets;

namespace Hyprx.Rex.Tasks;

public class SequentialTasksPipeline : Pipeline<SequentialTasksPipelineContext, TasksSummary>
{
    public SequentialTasksPipeline()
    {
        this.Use(new ExecuteTasksSequentialMiddleware());
    }

    public override async Task<TasksSummary> RunAsync(SequentialTasksPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.PipeAsync(context, cancellationToken);
            return new TasksSummary
            {
                Results = context.Results,
                Exception = context.Exception,
                Status = context.Status,
            };
        }
        catch (Exception ex)
        {
            context.Exception = ex;
            context.Status = RunStatus.Failed;
            return new TasksSummary
            {
                Results = context.Results,
                Exception = ex,
                Status = RunStatus.Failed,
            };
        }
    }
}

public class SequentialTasksPipelineContext : BusContext
{
    public SequentialTasksPipelineContext(RunContext context, IEnumerable<string> targets, TaskMap? map = null)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(targets, nameof(targets));
        this.Targets.AddRange(targets);
        this.Tasks = map ?? TaskMap.Global;
    }

    public TaskMap Tasks { get; }

    public List<TaskResult> Results { get; } = new List<TaskResult>();

    public RunStatus Status { get; set; } = RunStatus.Pending; // D

    public Exception? Exception { get; set; }

    public List<string> Targets { get; } = new List<string>();
}

public class ExecuteTasksSequentialMiddleware : IPipelineMiddleware<SequentialTasksPipelineContext>
{
    public async Task NextAsync(SequentialTasksPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var targets = context.Targets;
        var tasks = context.Tasks;
        var cyclesResult = tasks.DetectCyclycalReferences();
        var bus = context.Bus;
        if (bus is null)
        {
            context.Exception = new InvalidOperationException("Message bus not available in the service provider.");
            context.Status = RunStatus.Failed;
            return;
        }

        if (cyclesResult.Count > 0)
        {
            context.Exception = new InvalidOperationException($"Cyclical dependencies detected: {string.Join(", ", cyclesResult)}");
            context.Status = RunStatus.Failed;
            await context.Bus.SendAsync(new TasksFoundCyclycalReferences(cyclesResult));
        }

        var missingDeps = tasks.DetectMissingDependencies();
        if (missingDeps.Count > 0)
        {
            context.Exception = new InvalidOperationException($"Missing dependencies detected: {string.Join(", ", missingDeps)}");
            context.Status = RunStatus.Failed;
            await context.Bus.SendAsync(new TasksFoundMissingDependencies(missingDeps));
        }

        var targetTasks = new List<CodeTask>();
        foreach (var target in targets)
        {
            if (tasks.TryGetValue(target, out var task))
            {
                targetTasks.Add(task);
            }
            else
            {
                context.Exception = new KeyNotFoundException($"Task '{target}' not found in the task map.");
                context.Status = RunStatus.Failed;
                return;
            }
        }

        var flattenedResult = tasks.Flatten(targetTasks);
        if (flattenedResult.IsError)
        {
            context.Exception = new InvalidOperationException("Failed to flatten task dependencies.");
            context.Status = RunStatus.Failed;
            return;
        }

        var set = flattenedResult.Value;
        var taskPipeline = context.Services.GetService(typeof(TaskPipeline)) as TaskPipeline ?? new TaskPipeline();
        foreach (var task in set)
        {
            var data = new CodeTaskData(task.Id);
            var nextCtx = new TaskPipelineContext(context, task, data);

            if (context.Status is RunStatus.Cancelled or RunStatus.Failed && task.Force.HasValue && !task.Force.Value)
            {
                nextCtx.Status = RunStatus.Skipped;
                nextCtx.Result.Skip();
                context.Results.Add(nextCtx.Result);
                continue;
            }

            var r = await taskPipeline.RunAsync(nextCtx, cancellationToken);

            context.Outputs.Add($"task.{task.Id}", r.Output);
            context.Outputs.Add(task.Id, r.Output);

            var masker = context.Services.GetService(typeof(ISecretMasker)) as ISecretMasker;

            foreach (var kvp in nextCtx.Secrets)
            {
                if (context.Secrets.TryGetValue(kvp.Key, out var existingValue))
                {
                    if (existingValue != kvp.Value)
                    {
                        masker?.Add(kvp.Value);
                        context.Secrets[kvp.Key] = kvp.Value; // Overwrite with the new value

                        var envName = kvp.Key.ScreamingSnakeCase();
                        context.Env[envName] = kvp.Value;
                    }
                }
                else
                {
                    masker?.Add(kvp.Value);
                    context.Secrets[kvp.Key] = kvp.Value;

                    var envName = kvp.Key.ScreamingSnakeCase();
                    context.Env[envName] = kvp.Value;
                }
            }

            foreach (var kvp in nextCtx.Env)
            {
                if (context.Env.TryGetValue(kvp.Key, out var existingValue))
                {
                    if (existingValue != kvp.Value)
                    {
                        context.Env[kvp.Key] = kvp.Value; // Overwrite with the new value
                    }
                }
                else
                {
                    context.Env[kvp.Key] = kvp.Value;
                }
            }

            context.Results.Add(r);
            if (context.Status != RunStatus.Failed)
            {
                if (r.Status is RunStatus.Failed)
                {
                    context.Status = RunStatus.Failed;
                    context.Exception = r.Error;

                    break;
                }

                if (r.Status is RunStatus.Cancelled)
                {
                    context.Status = RunStatus.Cancelled;
                    context.Exception = r.Error;

                    break;
                }
            }
        }

        await next();
    }
}