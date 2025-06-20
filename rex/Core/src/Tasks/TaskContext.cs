using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Tasks;

public class TaskContext : RunContext
{
    public TaskContext(RunContext ctx, CodeTaskData data)
        : base(ctx)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        this.Data = data;
    }

    public CodeTaskData Data { get; }
}