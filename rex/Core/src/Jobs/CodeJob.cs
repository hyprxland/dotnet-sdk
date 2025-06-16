using Hyprx.Results;
using Hyprx.Rex.Collections;
using Hyprx.Rex.Execution;
using Hyprx.Rex.Tasks;

namespace Hyprx.Rex.Jobs;

public class CodeJob : INeedful
{
    public CodeJob(string id)
    {
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.Name = id;
    }

    public string Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DeferredJobValue<StringMap> Env { get; set; } = new StringMap();

    public DeferredJobValue<int> Timeout { get; set; } = 0;

    public DeferredJobValue<bool> If { get; set; } = true;

    public DeferredJobValue<Inputs> With { get; set; } = new Inputs();

    public DeferredJobValue<bool> Force { get; set; } = false;

    public TaskMap Tasks { get; set; } = new();

    public DeferredJobValue<string> Cwd { get; set; } = string.Empty;

    public string[] Needs { get; set; } = [];
}

public class JobBuilder
{
    private readonly CodeJob job;

    private readonly TaskMap globalTasks;

    public JobBuilder(CodeJob job, TaskMap? globalTasks = null)
    {
        this.job = job;
        this.globalTasks = globalTasks ?? TaskMap.Global;
    }

    public CodeJob Build() => this.job;

    public JobBuilder WithEnv(DeferredJobValue<StringMap> env)
    {
        this.job.Env = env;
        return this;
    }

    public JobBuilder WithName(string name)
    {
        this.job.Name = name;
        return this;
    }

    public JobBuilder WithDescription(string description)
    {
        this.job.Description = description;
        return this;
    }

    public JobBuilder Set(Action<CodeJob> action)
    {
        action(this.job);
        return this;
    }

    public JobBuilder WithIf(DeferredJobValue<bool> condition)
    {
        this.job.If = condition;
        return this;
    }

    public JobBuilder WithIf(bool condition)
    {
        this.job.If = condition;
        return this;
    }

    public JobBuilder WithTimeout(DeferredJobValue<int> timeout)
    {
        this.job.Timeout = timeout;
        return this;
    }

    public JobBuilder WithTimeout(int timeout)
    {
        this.job.Timeout = timeout;
        return this;
    }

    public JobBuilder WithCwd(DeferredJobValue<string> cwd)
    {
        this.job.Cwd = cwd;
        return this;
    }

    public JobBuilder WithCwd(string cwd)
    {
        this.job.Cwd = cwd;
        return this;
    }

    public JobBuilder WithTasks(Action<TaskMap> configure)
    {
        configure(this.job.Tasks);
        return this;
    }

    public CodeTask? GetGlobalTask(string id)
    {
        if (this.globalTasks.TryGetValue(id, out var task))
        {
            return task;
        }

        return null;
    }

    public JobBuilder AddGlobalTask(string id)
    {
        var task = this.GetGlobalTask(id);
        if (task != null)
        {
            this.job.Tasks[id] = task;
        }

        return this;
    }

    public TaskBuilder Task(string id, RunTaskAsync run)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, string[] needs, RunTaskAsync run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, Func<TaskContext, Task<Result<Outputs>>> run)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, string[] needs, Func<TaskContext, Task<Result<Outputs>>> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, Func<TaskContext, CancellationToken, Outputs> run)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, string[] needs, Func<TaskContext, CancellationToken, Outputs> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, string[] needs, Func<TaskContext, CancellationToken, Task<Outputs>> run)
    {
        var task = new DelegateTask(id, run);
        task.Needs = needs;
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, Func<TaskContext, CancellationToken, Task<Outputs>> run)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        return new TaskBuilder(task);
    }

    public TaskBuilder Task(string id, Action<TaskContext> run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return builder;
    }

    public TaskBuilder Task(string id, Action run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.job.Tasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return builder;
    }

    public JobBuilder WithNeeds(params string[] needs)
    {
        this.job.Needs = needs ?? [];
        return this;
    }
}