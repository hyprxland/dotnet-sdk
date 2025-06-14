namespace Hyprx.Dev.Deployments;

public interface IDeploymentEventHandler
{
    Task<DeploymentEventResult> RunAsync(DeploymentContext context, CancellationToken cancellationToken = default);
}