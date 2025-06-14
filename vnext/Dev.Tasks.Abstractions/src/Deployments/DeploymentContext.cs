using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Deployments;

public class DeploymentContext : RunContext
{
    public DeploymentContext(RunContext ctx, DeploymentData data)
        : base(ctx)
    {
        this.Data = data;
    }

    public DeploymentAction Action => this.Data.Action;

    public DeploymentData Data { get; }

    public DeploymentEventTasks EventTasks => this.Data.EventTasks;
}