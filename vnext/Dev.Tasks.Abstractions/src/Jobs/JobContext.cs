using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Jobs;

public class JobContext : RunContext
{
    public JobContext(RunContext context, JobData jobData)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(jobData, nameof(jobData));
        this.JobData = jobData;
    }

    public JobData JobData { get; }
}