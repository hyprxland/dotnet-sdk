using System.Reflection.Metadata.Ecma335;

using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Jobs;

public class JobPipeline : Pipeline<JobPipelineContext, JobResult>
{
    public JobPipeline()
    {
        this.Use(new RunJobMiddleware());
        this.Use(new ApplyJobContextMiddleware());
    }

    public override async Task<JobResult> RunAsync(JobPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                context.Result.Cancel();
                return context.Result;
            }

            await this.PipeAsync(context, cancellationToken);
            return context.Result;
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            context.Bus.Send(new JobFailed(context.JobData, ex));
            return context.Result;
        }
    }
}

public class SequentialJobsPipeline : Pipeline<JobsPipelineContext, JobsSummary>
{
    public SequentialJobsPipeline()
    {
        this.Use(new RunJobsSequentiallyMiddleware());
    }

    public override async Task<JobsSummary> RunAsync(JobsPipelineContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new JobsSummary
                {
                    Results = context.Results,
                    Status = RunStatus.Cancelled,
                    Exception = null,
                };
            }

            await this.PipeAsync(context, cancellationToken);
            return new JobsSummary
            {
                Results = context.Results,
                Status = context.Status,
                Exception = context.Exception,
            };
        }
        catch (Exception ex)
        {
            context.Status = RunStatus.Failed;
            context.Exception = ex;
            return new JobsSummary
            {
                Results = context.Results,
                Status = context.Status,
                Exception = context.Exception,
            };
        }
    }
}