using System.Runtime.Intrinsics.Arm;

using Hyprx.Extras;
using Hyprx.Results;
using Hyprx.Rex.Collections;

using Hyprx.Rex.Execution;
using Hyprx.Rex.Tasks;
using Hyprx.Secrets;

namespace Hyprx.Rex.Deployments;

public class ApplyDeploymentContextMiddleware : IPipelineMiddleware<DeploymentPipelineContext>
{
    public async Task NextAsync(DeploymentPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.Data;
        var deployment = context.Deployment;
        var action = data.Action;

        var deploymentContext = new DeploymentContext(context, data);

        try
        {
            foreach (var kvp in context.Env)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!deployment.Cwd.HasValue)
                await deployment.Cwd.ResolveAsync(deploymentContext, cancellationToken);

            data.Cwd = deployment.Cwd.Value;
            if (string.IsNullOrEmpty(data.Cwd))
                data.Cwd = context.Cwd ?? Environment.CurrentDirectory;

            if (!deployment.Timeout.HasValue)
                await deployment.Timeout.ResolveAsync(deploymentContext, cancellationToken);

            data.Timeout = deployment.Timeout.Value;

            if (!deployment.Force.HasValue)
                await deployment.Force.ResolveAsync(deploymentContext, cancellationToken);

            data.Force = deployment.Force.Value;

            if (!deployment.If.HasValue)
                await deployment.If.ResolveAsync(deploymentContext, cancellationToken);

            data.If = deployment.If.Value;

            if (!deployment.Env.HasValue)
                await deployment.Env.ResolveAsync(deploymentContext, cancellationToken);

            foreach (var kvp in deployment.Env.Value)
            {
                data.Env[kvp.Key] = kvp.Value;
            }

            if (!deployment.With.HasValue)
                await deployment.With.ResolveAsync(deploymentContext, cancellationToken);

            if (deployment.With.HasValue)
            {
                data.Inputs = new Inputs();

                var w = deployment.With.Value;
                if (w is null)
                    throw new InvalidOperationException("With cannot be null.");
                foreach (var kvp in w)
                {
                    data.Inputs[kvp.Key] = kvp.Value;
                }
            }

            data.EventHandlers = deployment.EventHandlers;
            data.EventTasks = deployment.EventTasks;
            data.Needs = deployment.Needs;

            await next();
        }
        catch (Exception ex)
        {
            context.Status = RunStatus.Failed;
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new DeployFailed(context.Data, context.Action, ex));
            return;
        }
    }
}

public class ExecuteDeploymentMiddleware : IPipelineMiddleware<DeploymentPipelineContext>
{
    public async Task NextAsync(DeploymentPipelineContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var data = context.Data;
        var deployment = context.Deployment;
        var action = data.Action;

        if (cancellationToken.IsCancellationRequested)
        {
            context.Status = RunStatus.Cancelled;
            context.Result.Cancel();
            await context.Bus.SendAsync(new DeployCancelled(data, action));
            return;
        }

        if (context.Status is RunStatus.Failed || (
            context.Status is RunStatus.Cancelled && !data.Force))
        {
            context.Result.Skip();
            await context.Bus.SendAsync(new DeploySkipped(data, action));
            return;
        }

        if (!data.If)
        {
            context.Status = RunStatus.Skipped;
            context.Result.Skip();
            await context.Bus.SendAsync(new DeploySkipped(data, action));
            return;
        }

        var defaults = context.Services.GetService(typeof(ExecutionDefaults)) as ExecutionDefaults ?? new ExecutionDefaults();
        var timeout = data.Timeout > 0 ? data.Timeout : defaults.Timeout;
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        context.Result.Start();
        if (cts.IsCancellationRequested)
        {
            context.Result.Cancel();
            await context.Bus.SendAsync(new DeployCancelled(data, action));
            return;
        }

        var masker = context.Services.GetService(typeof(ISecretMasker)) as ISecretMasker;

        foreach (var kvp in data.EventTasks)
        {
            var tasks = kvp.Value;
            var key = kvp.Key;

            if (tasks.Count == 0)
                continue;

            data.EventHandlers[key] = new DelegateDeploymentEventHandler(async (ctx, ct) =>
            {
                var map = tasks;
                var targets = map.Values.Select(o => o.Id).ToArray();
                var tasksCtx = new SequentialTasksPipelineContext(ctx, targets, map);

                var pipelines = ctx.Services.GetService(typeof(SequentialTasksPipeline)) as SequentialTasksPipeline ?? new SequentialTasksPipeline();
                var summary = await pipelines.RunAsync(tasksCtx, ct);

                foreach (var kvp in tasksCtx.Secrets)
                {
                    if (!context.Secrets.ContainsKey(kvp.Key))
                    {
                        context.Secrets[kvp.Key] = kvp.Value;
                        masker?.Add(kvp.Value);
                        var name = kvp.Key.ScreamingSnakeCase();
                        context.Env[name] = kvp.Value;
                    }
                    else
                    {
                        var old = context.Secrets[kvp.Key];
                        if (old != kvp.Value)
                        {
                            context.Secrets[kvp.Key] = kvp.Value;
                            masker?.Add(kvp.Value);
                            var name = kvp.Key.ScreamingSnakeCase();
                            context.Env[name] = kvp.Value;
                        }
                    }
                }

                foreach (var kvp in tasksCtx.Env)
                {
                    if (!context.Env.ContainsKey(kvp.Key))
                    {
                        context.Env[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        var old = context.Env[kvp.Key];
                        if (old != kvp.Value)
                        {
                            context.Env[kvp.Key] = kvp.Value;
                        }
                    }
                }

                var res = new DeploymentEventResult(context.Data.Id, key);
                res.TaskResults.AddRange(summary.Results);

                switch (summary.Status)
                {
                    case RunStatus.Success:
                        res.Ok();
                        break;
                    case RunStatus.Skipped:
                        res.Skip();
                        break;
                    case RunStatus.Cancelled:
                        res.Cancel();
                        break;
                    case RunStatus.Failed:
                        res.Fail(summary.Exception ?? new Exception("One or more tasks failed."));
                        break;
                }

                return res;
            });
        }

        try
        {
            context.Result.Start();

            if (cancellationToken.IsCancellationRequested)
            {
                context.Status = RunStatus.Cancelled;
                context.Result.Cancel();
                await context.Bus.SendAsync(new DeployCancelled(data, action));
                return;
            }

            await context.Bus.SendAsync(new DeployStarted(data, action));

            var deploymentContext = new DeploymentContext(context, data);

            Result<Outputs> result;
            if (deployment is IDeploymentHandler delegateDeploymentHandler)
            {
                result = await delegateDeploymentHandler.RunAsync(deploymentContext, cts.Token);
            }
            else
            {
                if (DeploymentHandlerMap.Global.TryGetValue(deployment.Id, out var handler))
                {
                    result = await handler.RunAsync(deploymentContext, cts.Token);
                }
                else
                {
                    var ex = new InvalidOperationException($"No handler found for deployment '{deployment.Id}'.");
                    result = new Result<Outputs>(ex);
                }
            }

            if (result.IsOk)
            {
                context.Result.Ok(result.Value);
                await context.Bus.SendAsync(new DeployCompleted(data, data.Action));
            }
            else
            {
                context.Result.Fail(result.Error);
                await context.Bus.SendAsync(new DeployFailed(data, data.Action, result.Error));
            }
        }
        catch (Exception ex)
        {
            cts.Dispose();

            // Handle any exceptions that occur during task execution
            context.Result.Fail(ex);
            await context.Bus.SendAsync(new TaskFailed(data, ex));
        }

        await next();
    }
}