using Hyprx;
using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;
using Hyprx.Results;

namespace Hyprx.Dev.Deployments;

public class DelegateDeployment : CodeDeployment
{
    private readonly Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> deployAsync;

    private readonly Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? rollbackAsync;

    private readonly Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? destroyAsync;

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> deployAsync,
        Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? rollbackAsync = null,
        Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? destroyAsync = null)
    {
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.Name = id;
        this.deployAsync = deployAsync ?? throw new ArgumentNullException(nameof(deployAsync));
        this.rollbackAsync = rollbackAsync;
        this.destroyAsync = destroyAsync;
    }

    public async Task<Result<Outputs>> RunAsync(
        DeploymentContext context,
        CancellationToken cancellationToken = default)
    {
        var data = context.Data;

        var before = $"before:{data.Action}";
        var after = $"after:{data.Action}";

        if (data.EventHandlers.ContainsKey(before) && data.EventTasks.HasTasks(before))
        {
            var handler = data.EventHandlers[before];
            var r = await handler.RunAsync(context, cancellationToken).ConfigureAwait(false);
            if (r.Error is not null || r.Status == RunStatus.Failed ||
                r.Status == RunStatus.Cancelled)
            {
                return new InvalidOperationException(
                    $"Deployment ${context.Data.Id} failed within event '{before}'", r.Error);
            }
        }

        Result<Outputs> result = new ResourceNotFoundException(
            $"Deployment action '{data.Action.Name}' is not supported by this deployment type.");

        switch (data.Action.Name)
        {
            case "deploy":
                if (this.deployAsync is not null)
                {
                    result = await this.deployAsync(context, cancellationToken).ConfigureAwait(false);
                }

                break;
            case "rollback":
                if (this.rollbackAsync is not null)
                {
                    result = await this.rollbackAsync(context, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return new ResourceNotFoundException(
                        $"Deployment action '{data.Action.Name}' is not supported for deployment {context.Data.Id}.");
                }

                break;
            case "destroy":
                if (this.destroyAsync is not null)
                {
                    result = await this.destroyAsync(context, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return new ResourceNotFoundException(
                        $"Deployment action '{data.Action.Name}' is not supported for deployment {context.Data.Id}.");
                }

                break;
            default:
                return new ResourceNotFoundException(
                    $"Deployment action '{data.Action.Name}' is not supported for deployment {context.Data.Id}.");
        }

        if (result.IsError)
            return result.Error;

        if (data.EventHandlers.ContainsKey(after) && data.EventTasks.HasTasks(after))
        {
            var handler = data.EventHandlers[after];
            var r = await handler.RunAsync(context, cancellationToken).ConfigureAwait(false);
            if (r.Error is not null || r.Status == RunStatus.Failed ||
                r.Status == RunStatus.Cancelled)
            {
                return new InvalidOperationException(
                    $"Deployment ${context.Data.Id} failed within event '{after}'", r.Error);
            }
        }

        return result;
    }
}