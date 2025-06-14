using Hyprx.Dev.Execution;
using Hyprx.Dev.Tasks;

namespace Hyprx.Dev.Deployments;

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