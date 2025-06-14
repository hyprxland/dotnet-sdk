using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Tasks;

public class TasksSummary
{
    public List<TaskResult> Results { get; set;  } = new List<TaskResult>();

    public Exception? Exception { get; set; }

    public RunStatus Status { get; set; } = RunStatus.Pending; // Default status is Pending
}