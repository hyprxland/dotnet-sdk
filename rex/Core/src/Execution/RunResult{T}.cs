using Hyprx.Rex.Collections;

namespace Hyprx.Rex.Execution;

public class RunResult<T>
    where T : RunResult<T>
{
    public RunResult(string id)
    {
        this.Id = id;
        this.Status = RunStatus.Pending;
        this.Output = new Outputs();
    }

    public string Id { get; set; }

    public RunStatus Status { get; private set; }

    public Outputs Output { get; private set; }

    public DateTimeOffset StartTime { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset EndTime { get; set; } = DateTimeOffset.MinValue;

    public TimeSpan Duration => this.EndTime - this.StartTime;

    public Exception? Error { get; set; }

    public T Start()
    {
        this.StartTime = DateTimeOffset.UtcNow;
        this.Status = RunStatus.Running;
        this.Error = null;
        return (T)this;
    }

    public T Ok()
    {
        this.EndTime = DateTimeOffset.UtcNow;
        this.Status = RunStatus.Success;
        this.Error = null;
        return (T)this;
    }

    public T Ok(Outputs output)
    {
        this.Output = output;
        return this.Ok();
    }

    public T Fail(Exception? error = null)
    {
        this.EndTime = DateTimeOffset.UtcNow;
        this.Status = RunStatus.Failed;
        this.Error = error;
        return (T)this;
    }

    public T Skip()
    {
        if (this.StartTime > DateTimeOffset.MinValue)
            this.EndTime = DateTimeOffset.UtcNow;

        this.Status = RunStatus.Skipped;
        this.Error = null;
        return (T)this;
    }

    public T Cancel()
    {
        if (this.StartTime > DateTimeOffset.MinValue)
            this.EndTime = DateTimeOffset.UtcNow;

        this.Status = RunStatus.Cancelled;
        this.Error = null;
        return (T)this;
    }
}