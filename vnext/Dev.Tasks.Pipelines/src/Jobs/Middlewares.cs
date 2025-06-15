using Hyprx.Dev.Collections;
using Hyprx.Dev.Execution;
using Hyprx.Dev.Tasks;
using Hyprx.Extras;

namespace Hyprx.Dev.Jobs;

public class ApplyJobContextMiddleware : IPipelineMiddleware<JobPipelineContext>
{
    public async Task NextAsync(JobPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.JobData;
        var job = context.Job;

        try
        {
            foreach (var kvp in context.Env)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!job.Cwd.HasValue)
                await job.Cwd.ResolveAsync(context, cancellationToken);

            var cwd = job.Cwd.Value;
            data.Cwd = !string.IsNullOrEmpty(cwd) ? cwd : Environment.CurrentDirectory;

            if (!job.Timeout.HasValue)
                await job.Timeout.ResolveAsync(context, cancellationToken);

            data.Timeout = job.Timeout.Value;

            if (!job.Force.HasValue)
                await job.Force.ResolveAsync(context, cancellationToken);

            data.Force = job.Force.Value;

            if (!job.If.HasValue)
                await job.If.ResolveAsync(context, cancellationToken);

            data.If = job.If.Value;

            if (!job.Env.HasValue)
                await job.Env.ResolveAsync(context, cancellationToken);

            var env = job.Env.Value;
            foreach (var kvp in env)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!job.With.HasValue)
                await job.With.ResolveAsync((RunContext)context, cancellationToken);

            data.Inputs = job.With.Value;

            data.Tasks = job.Tasks;

            await next();
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new JobFailed(data, ex));
        }
    }
}

public class RunJobMiddleware : IPipelineMiddleware<JobPipelineContext>
{

    public async Task NextAsync(JobPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.JobData;
        var job = context.Job;

        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                context.Result.Cancel();
                await context.Bus.SendAsync(new JobCancelled(data));
                return;
            }

            if (context.Status == RunStatus.Failed || (context.Status is RunStatus.Cancelled && !data.Force))
            {
                context.Result.Skip();
                await context.Bus.SendAsync(new JobSkipped(data));
                return;
            }

            if (context.JobData.Tasks.Count == 0)
            {
                var ex = new InvalidOperationException($"Job '{job.Id}' has no tasks to run.");
                context.Result.Fail(ex);
                await context.Bus.SendAsync(new JobFailed(data, ex));
                return;
            }

            if (data.If == false)
                {
                    context.Result.Skip();
                    await context.Bus.SendAsync(new JobSkipped(data));
                    return;
                }

            var timeout = data.Timeout;
            if (timeout < 0)
            {
                var defaults = context.GetService<ExecutionDefaults>();
                if (defaults == null)
                {
                    defaults = new ExecutionDefaults();
                }

                timeout = defaults.Timeout;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (timeout > 0)
            {
                cts.CancelAfter(timeout);
            }

            var token = cts.Token;

            try
            {
                context.Result.Start();
                await context.Bus.SendAsync(new JobStarted(data));

                var pipeline = context.GetService<SequentialTasksPipeline>();
                pipeline ??= new SequentialTasksPipeline();

                var targets = data.Tasks.Values.Select(t => t.Id).ToList();
                var ctx = new SequentialTasksPipelineContext(context, targets, data.Tasks);

                var summary = await pipeline.RunAsync(ctx, token);
                if (summary.Exception is not null)
                {
                    context.Result.Fail(summary.Exception);
                    await context.Bus.SendAsync(new JobFailed(data, summary.Exception));
                    return;
                }

                var normalized = job.Id.Underscore();

                foreach (var kvp in ctx.Outputs)
                {
                    if (kvp.Key.StartsWith("task.", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Outputs[$"job.{normalized}.{kvp.Key}"] = kvp.Value;
                    }
                }

                context.Result.Ok();
                await context.Bus.SendAsync(new JobCompleted(data));
            }
            catch (Exception ex)
            {
                context.Result.Fail(ex);
                await context.Bus.SendAsync(new JobFailed(data, ex));
                return;
            }
            finally
            {
                cts.Dispose();
            }

            await next();
        }
        catch (Exception ex)
        {
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new JobFailed(data, ex));
        }
    }
}

public class RunJobsSequentiallyMiddleware : IPipelineMiddleware<JobsPipelineContext>
{
    public async Task NextAsync(JobsPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var jobs = context.Jobs;
        var targets = context.Targets;

        var cycles = jobs.DetectCyclycalReferences();
        if (cycles.Count > 0)
        {
            context.Status = RunStatus.Failed;
            context.Exception = new InvalidOperationException($"Cyclical job references detected: {string.Join(", ", cycles)}");

            await context.Bus.SendAsync(new JobFoundCyclycalReferences(cycles));
            return;
        }

        var missingDeps = jobs.DetectMissingDependencies();
        if (missingDeps.Count > 0)
        {
            context.Status = RunStatus.Failed;
            context.Exception = new InvalidOperationException($"Missing job dependencies detected: {string.Join(", ", missingDeps.Select(kvp => $"{kvp.item} -> {string.Join(", ", kvp.missing)}"))}");

            await context.Bus.SendAsync(new JobFoundMissingDependencies(missingDeps));
            return;
        }

        var jobsMap = new JobMap();
        foreach (var target in targets)
        {
            if (jobs.TryGetValue(target, out var job))
            {
                jobsMap[target] = job;
            }
            else
            {
                context.Status = RunStatus.Failed;
                context.Exception = new KeyNotFoundException($"Job '{target}' not found.");
                return;
            }
        }

        var flattened = jobs.Flatten(jobsMap.Values.ToList());
        if (flattened.IsError)
        {
            context.Status = RunStatus.Failed;
            context.Exception = flattened.Error;
            return;
        }

        var jobSet = flattened.Value;

        var outputs = new Outputs(context.Outputs);

        foreach (var job in jobSet)
        {
            var pipeline = context.GetService<JobPipeline>() ?? new JobPipeline();
            var nextContext = new JobPipelineContext(context, job, new CodeJobData() { Id = job.Id, Name = job.Name ?? job.Id, });

            var result = await pipeline.RunAsync(nextContext, cancellationToken);

            foreach (var kvp in nextContext.Outputs)
            {
                if (kvp.Key.StartsWith("jobs.", StringComparison.OrdinalIgnoreCase))
                {
                    if (!context.Outputs.ContainsKey(kvp.Key))
                    {
                        context.Outputs[kvp.Key] = kvp.Value;
                    }
                }
            }

            context.Results.Add(result);

            if (context.Status is not RunStatus.Failed)
            {
                if (result.Status is RunStatus.Failed)
                {
                    context.Status = RunStatus.Failed;
                    context.Exception = result.Error;
                }
                else if (result.Status is RunStatus.Cancelled)
                {
                    context.Status = RunStatus.Cancelled;
                }
            }
        }

        await next();
    }
}