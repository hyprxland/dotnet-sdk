using Hyprx.Rex.Jobs;
using Hyprx.Rex.Messaging;

namespace Hyprx.Rex.Jobs;

public abstract class JobMessageBase : IMessage
{
    public JobMessageBase(string kind, CodeJobData data)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(kind, nameof(kind));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        this.Topic = kind;
        this.Data = data;
    }

    public string Topic { get; }

    public CodeJobData Data { get; }

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}

public class JobStarted : JobMessageBase
{
    public JobStarted(CodeJobData data)
        : base("job:started", data)
    {
    }
}

public class JobFailed : JobMessageBase
{
    public JobFailed(CodeJobData data, Exception exception)
        : base("job:failed", data)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));
        this.Exception = exception;
    }

    public Exception Exception { get; }
}

public class JobSkipped : JobMessageBase
{
    public JobSkipped(CodeJobData data)
        : base("job:skipped", data)
    {
    }
}

public class JobCancelled : JobMessageBase
{
    public JobCancelled(CodeJobData data)
        : base("job:cancelled", data)
    {
    }
}

public class JobCompleted : JobMessageBase
{
    public JobCompleted(CodeJobData data)
        : base("job:completed", data)
    {
    }
}

public class JobFoundCyclycalReferences : IMessage
{
    public JobFoundCyclycalReferences(List<CodeJob> job)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        this.Job.AddRange(job);
    }

    public string Topic => "jobs:cyclical-references";

    public List<CodeJob> Job { get; } = [];
}

public class JobFoundMissingDependencies : IMessage
{
    public JobFoundMissingDependencies(List<(CodeJob, List<string>)> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks, nameof(tasks));
        this.Tasks.AddRange(tasks);
    }

    public string Topic => "jobs:missing-dependencies";

    public List<(CodeJob, List<string>)> Tasks { get; } = new();
}