using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeploymentsSummary
{
    public List<DeploymentResult> Results { get; set; } = new List<DeploymentResult>();

    public Exception? Exception { get; set; }

    public RunStatus Status { get; set; } = RunStatus.Pending;
}