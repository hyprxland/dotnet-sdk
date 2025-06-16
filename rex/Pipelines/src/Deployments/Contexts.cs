using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeploymentPipelineContext : BusContext
{
    public DeploymentPipelineContext(RunContext context, CodeDeployment deployment, CodeDeploymentData data)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(deployment, nameof(deployment));
        this.Data = data;
        this.Deployment = deployment;
        this.Result = new DeploymentResult(deployment.Id);
    }

    public DeploymentResult Result { get; }

    public CodeDeploymentData Data { get; }

    public CodeDeployment Deployment { get; }

    public DeploymentAction Action => this.Data.Action;

    public RunStatus Status { get; set; } = RunStatus.Pending;
}

public class DeploymentsPipelineContext : BusContext
{
    public DeploymentsPipelineContext(RunContext context, string[] targets, DeploymentAction action, DeploymentMap? deployments = null)
        : base(context)
    {
        this.Targets = targets;
        this.Deployments = deployments ?? DeploymentMap.Default;
    }

    public DeploymentMap Deployments { get; } = new DeploymentMap();

    public DeploymentAction Action { get; set; }

    public List<DeploymentResult> Results { get; } = new List<DeploymentResult>();

    public RunStatus Status { get; set; } = RunStatus.Pending;

    public Exception? Exception { get; set; }

    public string[] Targets { get; }
}