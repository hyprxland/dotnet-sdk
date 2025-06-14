using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Tasks;

public class TaskResult : RunResult<TaskResult>
{
    public TaskResult(string id)
        : base(id)
    {
    }
}