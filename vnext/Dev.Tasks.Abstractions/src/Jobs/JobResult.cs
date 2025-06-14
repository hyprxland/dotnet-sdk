using Hyprx.Dev.Execution;
using Hyprx.Dev.Tasks;

namespace Hyprx.Dev.Jobs;

public class JobResult : RunResult<JobResult>
{
    public JobResult(string id)
        : base(id)
    {
        this.TaskResults = new List<TaskResult>();
    }

    public List<TaskResult> TaskResults { get; }
}