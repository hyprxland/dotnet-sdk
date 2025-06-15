using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;
using Hyprx.Dev.Jobs;
using Hyprx.Dev.Messaging;
using Hyprx.Lodi;
using Hyprx.Results;

namespace Hyprx.Dev.Tasks.Runners;

public static class ConsoleRunner
{
    public static TaskMap Tasks { get; } = TaskMap.Global;

    public static JobMap Jobs { get; } = JobMap.Global;

    public static CodeTask Task(CodeTask task)
    {
        Tasks[task.Id] = task;
        return task;
    }

    public static CodeTask Task(string id, RunTaskAsync run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, RunTaskAsync run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, Func<TaskContext, Task<Result<Outputs>>> run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, Func<TaskContext, Task<Result<Outputs>>> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, Func<TaskContext, CancellationToken, Outputs> run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, Func<TaskContext, CancellationToken, Outputs> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, Func<TaskContext, CancellationToken, Task<Outputs>> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, Func<TaskContext, CancellationToken, Task<Outputs>> run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, Action<TaskContext> run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, Action<TaskContext> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, Action run)
    {
        var task = new DelegateTask(id, run);
        Tasks[id] = task;
        return task;
    }

    public static CodeTask Task(string id, string[] needs, Action run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        Tasks[id] = task;
        return task;
    }

    public static async Task<int> RunAsync(ConsoleRunnerOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (options.Services is null)
        {
            var sp = new LodiServiceProvider();
            sp.RegisterSingleton<TaskPipeline>(() => new TaskPipeline());
            sp.RegisterSingleton<SequentialTasksPipeline>(() => new SequentialTasksPipeline());
            sp.RegisterSingleton<JobPipeline>(() => new JobPipeline());
            sp.RegisterSingleton<SequentialJobsPipeline>(() => new SequentialJobsPipeline());
            sp.RegisterSingleton<ExecutionDefaults>(() => new ExecutionDefaults { Timeout = 60 * 60 * 1000 }); // Default timeout is 1 hour
            sp.RegisterSingleton<IMessageBus>(() => new ConsoleMessageBus());

            options.Services = sp;
        }

        var serviceProvider = options.Services;
        var defaults = serviceProvider.GetService(typeof(ExecutionDefaults)) as ExecutionDefaults;
        if (defaults is not null)
        {
            defaults.Timeout = options.Timeout > 0 ? options.Timeout : 0;
        }

        var globalTasks = options.Tasks;
        var globalJobs = options.Jobs;

        if (globalTasks.Count == 0 && globalJobs.Count == 0)
        {
            Console.WriteLine("No tasks or jobs defined.");
            return 1;
        }

        if (options.Cmd == "auto")
        {
            var first = options.Targets.FirstOrDefault();
            if (first is null)
            {
                Console.WriteLine("No targets specified.");
                return 1;
            }

            if (globalTasks.ContainsKey(first))
            {
                options.Cmd = "task";
            }
            else if (globalJobs.ContainsKey(first))
            {
                options.Cmd = "job";
            }
            else
            {
                Console.WriteLine($"Target '{first}' not found in tasks or jobs.");
                return 1;
            }
        }

        var runContext = new RunContext(options.Context, serviceProvider);

        switch (options.Cmd)
        {
            case "task":
                {
                    var ctx = new SequentialTasksPipelineContext(runContext, options.Targets, options.Tasks);
                    var pipeline = serviceProvider.GetService(typeof(SequentialTasksPipeline)) as SequentialTasksPipeline ?? new SequentialTasksPipeline();
                    var summary = await pipeline!.RunAsync(ctx, cancellationToken);
                    if (summary.Exception is not null)
                    {
                        Console.WriteLine($"Task execution failed: {summary.Exception}");
                        return 1;
                    }

                    if (summary.Status == RunStatus.Failed)
                    {
                        Console.WriteLine("One or more tasks failed.");
                        return 1;
                    }

                    return 0;
                }

            case "job":
                {
                    var ctx = new JobsPipelineContext(runContext, options.Targets, options.Jobs);
                    var pipeline = serviceProvider.GetService(typeof(SequentialJobsPipeline)) as SequentialJobsPipeline;
                    var summary = await pipeline!.RunAsync(ctx, cancellationToken);
                    if (summary.Exception is not null)
                    {
                        Console.WriteLine($"Job execution failed: {summary.Exception.Message} \n{summary.Exception.StackTrace}");
                        return 1;
                    }

                    if (summary.Status == RunStatus.Failed)
                    {
                        Console.WriteLine("One or more jobs failed.");
                        return 1;
                    }

                    return 0;
                }

            case "list":
                {
                    if (globalTasks.Count > 0)
                    {
                        Console.WriteLine("Tasks:");
                        foreach (var kvp in globalTasks)
                        {
                            Console.WriteLine($"- {kvp.Key}: {kvp.Value.Description}");
                        }
                    }

                    if (globalJobs.Count > 0)
                    {
                        Console.WriteLine("Jobs:");
                        foreach (var kvp in globalJobs)
                        {
                            Console.WriteLine($"- {kvp.Key}: {kvp.Value.Description}");
                        }
                    }
                }

                break;

            default:
                Console.WriteLine($"Unknown command '{options.Cmd}'.");
                return 1;
        }

        return 1;
    }
}