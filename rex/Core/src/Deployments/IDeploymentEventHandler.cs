namespace Hyprx.Rex.Deployments;

public interface IDeploymentEventHandler
{
    Task<DeploymentEventResult> RunAsync(DeploymentContext context, CancellationToken cancellationToken = default);
}

public class DelegateDeploymentEventHandler : IDeploymentEventHandler
{
    private readonly Func<DeploymentContext, CancellationToken, Task<DeploymentEventResult>> handler;

    public DelegateDeploymentEventHandler(Func<DeploymentContext, CancellationToken, Task<DeploymentEventResult>> handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task<DeploymentEventResult> RunAsync(DeploymentContext context, CancellationToken cancellationToken = default)
    {
        return this.handler(context, cancellationToken);
    }
}