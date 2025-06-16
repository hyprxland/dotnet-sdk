using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Deployments;

public class DeploymentPipeline : Pipeline<DeploymentPipelineContext, DeploymentResult>
{
    public DeploymentPipeline()
    {
        this.Use(new ExecuteDeploymentMiddleware());
        this.Use(new ApplyDeploymentContextMiddleware());
    }

    public override async Task<DeploymentResult> RunAsync(DeploymentPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.PipeAsync(context, cancellationToken).ConfigureAwait(false);
            return context.Result;
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            var data = context.Data;
            var action = context.Action;
            await context.Bus.SendAsync(new DeployFailed(data, action, ex));
            throw;
        }
    }
}

public class SequentialDeploymentsPipeline : Pipeline<DeploymentsPipelineContext, DeploymentsSummary>
{
    public SequentialDeploymentsPipeline()
    {
    }

    public override async Task<DeploymentsSummary> RunAsync(DeploymentsPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await this.PipeAsync(context, cancellationToken).ConfigureAwait(false);
            return new DeploymentsSummary()
            {
                Status = context.Status,
                Exception = context.Exception,
                Results = context.Results,
            };
        }
        catch (Exception ex)
        {
            return new DeploymentsSummary()
            {
                Status = RunStatus.Failed,
                Exception = ex,
                Results = context.Results,
            };
        }
    }
}