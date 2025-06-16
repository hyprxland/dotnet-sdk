using Hyprx.Rex.Execution;

namespace Hyprx.Rex.Jobs;

public class JobContext : Execution.RunContext
{
    public JobContext(Execution.RunContext context, CodeJobData jobData)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(jobData, nameof(jobData));
        this.JobData = jobData;
    }

    public CodeJobData JobData { get; }
}