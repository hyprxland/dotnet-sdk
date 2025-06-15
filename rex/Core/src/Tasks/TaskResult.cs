using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Tasks;

public class TaskResult : RunResult<TaskResult>
{
    public TaskResult(string id)
        : base(id)
    {
    }
}