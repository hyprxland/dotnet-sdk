using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeploymentContext : RunContext
{
    public DeploymentContext(RunContext ctx, CodeDeploymentData data)
        : base(ctx)
    {
        this.Data = data;
    }

    public DeploymentAction Action => this.Data.Action;

    public CodeDeploymentData Data { get; }

    public DeploymentEventTasks EventTasks => this.Data.EventTasks;
}