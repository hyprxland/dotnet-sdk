using Hyprx.Rex.Execution;
using Hyprx.Rex.Tasks;

namespace Hyprx.Rex.Deployments;

public class DeploymentEventResult : RunResult<DeploymentEventResult>
{
    public DeploymentEventResult(string id, string eventName)
        : base(id)
    {
        this.EventName = eventName;
    }

    public List<TaskResult> TaskResults { get; } = new List<TaskResult>();

    public string EventName { get; }
}