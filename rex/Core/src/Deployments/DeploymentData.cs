using Hyprx.Rex.Tasks;

namespace Hyprx.Rex.Deployments;

public class DeploymentData : CodeTaskData
{
    public DeploymentData(string id)
        : base(id)
    {
    }

    public DeploymentAction Action { get; set; } = DeploymentAction.Deploy;

    public DeploymentEventTasks EventTasks { get; set; } = new DeploymentEventTasks();

    public Dictionary<string, IDeploymentEventHandler> EventHandlers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}