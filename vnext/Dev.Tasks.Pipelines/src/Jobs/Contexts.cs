using Hyprx.Dev.Execution;

namespace Hyprx.Dev.Jobs;

public class JobPipelineContext : BusContext
{
    public JobPipelineContext(Execution.RunContext context, CodeJob codeJob, CodeJobData jobData)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(codeJob, nameof(codeJob));
        ArgumentNullException.ThrowIfNull(jobData, nameof(jobData));
        this.Job = codeJob;
        this.JobData = jobData;
        this.Result = new JobResult(codeJob.Id);
    }

    public CodeJob Job { get; set; }

    public CodeJobData JobData { get; set; }

    public JobResult Result { get; set; }

    public RunStatus Status { get; set; } = RunStatus.Pending;
}

public class JobsPipelineContext : BusContext
{
    public JobsPipelineContext(Execution.RunContext context, string[] targets, JobMap? jobs = null)
        : base(context)
    {

        this.Jobs = jobs ?? JobMap.Global;
        this.Targets = targets ?? Array.Empty<string>();
    }

    public JobMap Jobs { get; }

    public string[] Targets { get; set; }

    public List<JobResult> Results { get; set; } = [];

    public RunStatus Status { get; set; } = RunStatus.Pending;

    public Exception? Exception { get; set; }
}

public class JobsSummary
{
    public List<JobResult> Results { get; set; } = [];

    public RunStatus Status { get; set; } = RunStatus.Pending;

    public Exception? Exception { get; set; }
}