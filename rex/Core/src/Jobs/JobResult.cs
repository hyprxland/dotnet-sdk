using Hyprx.Rex.Execution;
using Hyprx.Rex.Tasks;

namespace Hyprx.Rex.Jobs;

public class JobResult : RunResult<JobResult>
{
    public JobResult(string id)
        : base(id)
    {
        this.TaskResults = new List<TaskResult>();
    }

    public List<TaskResult> TaskResults { get; }
}