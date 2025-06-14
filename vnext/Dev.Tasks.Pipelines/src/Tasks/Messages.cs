using Hyprx.Dev.Messaging;

namespace Hyprx.Dev.Tasks;

public class TaskMessaseBase : IMessage
{
    public TaskMessaseBase(string kind, CodeTaskData data)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(kind, nameof(kind));
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        this.Topic = kind;
        this.Data = data;
    }

    public string Topic { get; }

    public CodeTaskData Data { get; }

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}

public class TaskStarted : TaskMessaseBase
{
    public TaskStarted(CodeTaskData data)
        : base("task:started", data)
    {
    }
}

public class TaskFailed : TaskMessaseBase
{
    public TaskFailed(CodeTaskData data, Exception exception)
        : base("task:failed", data)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));
        this.Exception = exception;
    }

    public Exception Exception { get; }
}

public class TaskSkipped : TaskMessaseBase
{
    public TaskSkipped(CodeTaskData data)
        : base("task:skipped", data)
    {
    }
}

public class TaskCancelled : TaskMessaseBase
{
    public TaskCancelled(CodeTaskData data)
        : base("task:cancelled", data)
    {
    }
}

public class TaskCompleted : TaskMessaseBase
{
    public TaskCompleted(CodeTaskData data)
        : base("task:completed", data)
    {
    }
}

public class FoundCyclycalReferences : IMessage
{
    public FoundCyclycalReferences(List<CodeTask> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks, nameof(tasks));
        this.Tasks.AddRange(tasks);
    }

    public string Topic => "tasks:cyclical-references";

    public List<CodeTask> Tasks { get; } = new();
}

public class FoundMissingDependencies : IMessage
{
    public FoundMissingDependencies(List<(CodeTask, List<string>)> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks, nameof(tasks));
        this.Tasks.AddRange(tasks);
    }

    public string Topic => "tasks:missing-dependencies";

    public List<(CodeTask, List<string>)> Tasks { get; } = new();
}