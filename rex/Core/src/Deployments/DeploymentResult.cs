using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeploymentResult : RunResult<DeploymentResult>
{
    public DeploymentResult(string id)
        : base(id)
    {
    }
}