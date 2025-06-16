using Hyprx.Results;
using Hyprx.Rex.Collections;
using Hyprx.Rex.Deployments;

public interface IDeploymentHandler
{
    /// <summary>
    /// Runs the task handler asynchronously.
    /// </summary>
    /// <param name="context">The task context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the task execution.</returns>
    Task<Result<Outputs>> RunAsync(DeploymentContext context, CancellationToken cancellationToken = default);
}