namespace Hyprx.Rex.Deployments;

public interface IDeploymentEventHandler
{
    Task<DeploymentEventResult> RunAsync(DeploymentContext context, CancellationToken cancellationToken = default);
}