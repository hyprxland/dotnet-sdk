using Hyprx;
using Hyprx.Results;
using Hyprx.Rex.Collections;
using Hyprx.Rex.Execution;
using Hyprx.Rex.Tasks;

using static Hyprx.Results.Result;

namespace Hyprx.Rex.Deployments;

public class DelegateDeployment : CodeDeployment, IDeploymentHandler
{
    private readonly Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> deployAsync;

    private Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? rollbackAsync;

    private Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>>? destroyAsync;

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> deployAsync)
    {
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.Name = id;
        this.deployAsync = deployAsync ?? throw new ArgumentNullException(nameof(deployAsync));
    }

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, Task<Result<Outputs>>> deployAsync)
        : this(id, (ctx, ct) => deployAsync(ctx))
    {
    }

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, CancellationToken, Outputs> deployAsync)
        : this(id, (ctx, ct) =>
        {
            try
            {
                var outputs = deployAsync(ctx, ct);
                var result = Ok(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        })
    {
    }

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, CancellationToken, Task<Outputs>> deployAsync)
        : this(id, async (ctx, ct) =>
        {
            try
            {
                var res = await deployAsync(ctx, ct);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Fail<Outputs>(ex);
            }
        })
    {
    }

    public DelegateDeployment(
        string id,
        Func<DeploymentContext, Outputs> deployAsync)
        : this(id, (ctx, ct) =>
        {
            try
            {
                var outputs = deployAsync(ctx);
                var result = Ok(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        })
    {
    }

    public DelegateDeployment(
        string id,
        Action<DeploymentContext> deployAsync)
        : this(id, (ctx, ct) =>
        {
            try
            {
                deployAsync(ctx);
                var result = Ok(new Outputs());
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        })
    {
    }

    public DelegateDeployment(
        string id,
        Action deployAsync)
        : this(id, (ctx, ct) =>
        {
            deployAsync();
            return Task.FromResult(Ok(new Outputs()));
        })
    {
    }

    public DelegateDeployment WithDestroy(Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> destroyAsync)
    {
        this.destroyAsync = destroyAsync ?? throw new ArgumentNullException(nameof(destroyAsync));
        return this;
    }

    public DelegateDeployment WithDestroy(Func<DeploymentContext, Task<Result<Outputs>>> run)
    {
        this.destroyAsync = (ctx, ct) => run(ctx);
        return this;
    }

    public DelegateDeployment WithDestroy(Func<DeploymentContext, CancellationToken, Outputs> run)
    {
        this.destroyAsync = (ctx, ct) =>
        {
            try
            {
                var outputs = run(ctx, ct);
                var result = Ok(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
        return this;
    }

    public DelegateDeployment WithDestroy(Func<DeploymentContext, CancellationToken, Task<Outputs>> run)
    {
        this.destroyAsync = async (ctx, ct) =>
        {
            try
            {
                var res = await run(ctx, ct);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Fail<Outputs>(ex);
            }
        };

        return this;
    }

    public DelegateDeployment WithDestroy(Action<DeploymentContext> run)
    {
        this.destroyAsync = (ctx, ct) =>
        {
            try
            {
                run(ctx);
                var result = Ok(new Outputs());
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
        return this;
    }

    public DelegateDeployment WithDestroy(Action destroyAsync)
    {
        this.destroyAsync = (ctx, ct) =>
        {
            destroyAsync();
            return Task.FromResult(Ok(new Outputs()));
        };
        return this;
    }

    public DelegateDeployment WithRollback(Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> rollbackAsync)
    {
        this.rollbackAsync = rollbackAsync ?? throw new ArgumentNullException(nameof(rollbackAsync));
        return this;
    }

    public DelegateDeployment WithRollback(Func<DeploymentContext, Task<Result<Outputs>>> run)
    {
        this.rollbackAsync = (ctx, ct) => run(ctx);
        return this;
    }

    public DelegateDeployment WithRollback(Func<DeploymentContext, CancellationToken, Outputs> run)
    {
        this.rollbackAsync = (ctx, ct) =>
        {
            try
            {
                var outputs = run(ctx, ct);
                var result = Ok(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
        return this;
    }

    public DelegateDeployment WithRollback(Func<DeploymentContext, CancellationToken, Task<Outputs>> run)
    {
        this.rollbackAsync = async (ctx, ct) =>
        {
            try
            {
                var res = await run(ctx, ct);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Fail<Outputs>(ex);
            }
        };

        return this;
    }

    public DelegateDeployment WithRollback(Action<DeploymentContext> run)
    {
        this.rollbackAsync = (ctx, ct) =>
        {
            try
            {
                run(ctx);
                var result = Ok(new Outputs());
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = Fail<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
        return this;
    }

    public DelegateDeployment WithRollback(Action rollbackAsync)
    {
        this.rollbackAsync = (ctx, ct) =>
        {
            rollbackAsync();
            return Task.FromResult(Ok(new Outputs()));
        };
        return this;
    }

    public async Task<Result<Outputs>> RunAsync(
        DeploymentContext context,
        CancellationToken cancellationToken = default)
    {
        var data = context.Data;

        var before = $"before:{data.Action.Name.ToLowerInvariant()}";
        var after = $"after:{data.Action.Name.ToLowerInvariant()}";
        if (data.EventHandlers.TryGetValue(before, out var beforeHandler))
        {
            var r = await beforeHandler.RunAsync(context, cancellationToken).ConfigureAwait(false);
            if (r.Error is not null || r.Status == RunStatus.Failed ||
                r.Status == RunStatus.Cancelled)
            {
                return new InvalidOperationException(
                    $"Deployment ${context.Data.Id} failed within event '{before}'", r.Error);
            }
        }
        else
        {
            Console.WriteLine($"No handler for event '{before}'");
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

        if (data.EventHandlers.TryGetValue(after, out var afterHandler))
        {
            var r = await afterHandler.RunAsync(context, cancellationToken).ConfigureAwait(false);
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

public class DeploymentBuilder
{
    private readonly CodeDeployment deployment;

    private readonly TaskMap globalTasks;

    public DeploymentBuilder(CodeDeployment deployment, TaskMap? globalTasks = null)
    {
        this.deployment = deployment ?? throw new ArgumentNullException(nameof(deployment));
        this.globalTasks = globalTasks ?? TaskMap.Global;
    }

    public DeploymentBuilder BeforeRollback(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.BeforeRollback);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        return this;
    }

    public DeploymentBuilder AfterRollback(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.AfterRollback);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        return this;
    }

    public DeploymentBuilder BeforeDestroy(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.BeforeDestroy);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        return this;
    }

    public DeploymentBuilder AfterDestroy(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.AfterDestroy);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        return this;
    }

    public DeploymentBuilder BeforeDeploy(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.BeforeDeployment);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        Console.WriteLine($"BeforeDeploy count is {map.Count}");
        return this;
    }

    public DeploymentBuilder AfterDeploy(Action<DeploymentEventBuilder> configure)
    {
        var map = this.deployment.EventTasks.GetOrCreateTaskMap(DeploymentEventNames.AfterDeployment);

        var builder = new DeploymentEventBuilder(map, this.globalTasks);
        configure(builder);
        return this;
    }

    public DeploymentBuilder WithDestroy(Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> destroyAsync)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(destroyAsync);
        }

        return this;
    }

    public DeploymentBuilder WithDestroy(Func<DeploymentContext, Task<Result<Outputs>>> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(run);
        }

        return this;
    }

    public DeploymentBuilder WithDestroy(Func<DeploymentContext, CancellationToken, Outputs> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(run);
        }

        return this;
    }

    public DeploymentBuilder WithDestroy(Func<DeploymentContext, CancellationToken, Task<Outputs>> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(run);
        }

        return this;
    }

    public DeploymentBuilder WithDestroy(Action<DeploymentContext> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(run);
        }

        return this;
    }

    public DeploymentBuilder WithDestroy(Action destroyAsync)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithDestroy(destroyAsync);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Func<DeploymentContext, CancellationToken, Task<Result<Outputs>>> rollbackAsync)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(rollbackAsync);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Func<DeploymentContext, Task<Result<Outputs>>> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(run);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Func<DeploymentContext, CancellationToken, Outputs> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(run);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Func<DeploymentContext, CancellationToken, Task<Outputs>> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(run);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Action<DeploymentContext> run)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(run);
        }

        return this;
    }

    public DeploymentBuilder WithRollback(Action rollbackAsync)
    {
        if (this.deployment is DelegateDeployment dd)
        {
            dd.WithRollback(rollbackAsync);
        }

        return this;
    }

    public DeploymentBuilder WithCwd(DeferredDeploymentValue<string> cwd)
    {
        this.deployment.Cwd = cwd;
        return this;
    }

    public DeploymentBuilder WithCwd(string cwd)
    {
        this.deployment.Cwd = cwd;
        return this;
    }

    public DeploymentBuilder WithDescription(string description)
    {
        this.deployment.Description = description;
        return this;
    }

    public DeploymentBuilder Set(Action<CodeDeployment> action)
    {
        action(this.deployment);
        return this;
    }

    public DeploymentBuilder WithIf(DeferredDeploymentValue<bool> condition)
    {
        this.deployment.If = condition;
        return this;
    }

    public DeploymentBuilder WithIf(bool condition)
    {
        this.deployment.If = condition;
        return this;
    }

    public DeploymentBuilder WithForce(DeferredDeploymentValue<bool> force)
    {
        this.deployment.Force = force;
        return this;
    }

    public DeploymentBuilder WithForce(bool force)
    {
        this.deployment.Force = force;
        return this;
    }

    public DeploymentBuilder WithNeeds(params string[] needs)
    {
        this.deployment.Needs = needs ?? Array.Empty<string>();
        return this;
    }
}

public class DeploymentEventBuilder
{
    private readonly TaskMap eventTasks;
    private readonly TaskMap globalTasks = TaskMap.Global;

    public DeploymentEventBuilder(TaskMap eventTasks, TaskMap? globalTasks = null)
    {
        this.eventTasks = eventTasks ?? throw new ArgumentNullException(nameof(eventTasks));
        if (globalTasks is not null)
        {
            this.globalTasks = globalTasks;
        }
    }

    public CodeTask? GetGlobalTask(string id)
    {
        if (this.globalTasks.TryGetValue(id, out var task))
        {
            return task;
        }

        return null;
    }

    public DeploymentEventBuilder AddGlobalTask(string id)
    {
        var task = this.GetGlobalTask(id);
        if (task != null)
        {
            this.eventTasks[id] = task;
        }

        return this;
    }

    public DeploymentEventBuilder Task(CodeTask task)
    {
        this.eventTasks[task.Id] = task;
        return this;
    }

    public DeploymentEventBuilder Task(string id, RunTaskAsync run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }

    public DeploymentEventBuilder Task(string id, Func<TaskContext, Task<Result<Outputs>>> run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }

    public DeploymentEventBuilder Task(string id, Func<TaskContext, CancellationToken, Outputs> run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }

    public DeploymentEventBuilder Task(string id, Func<TaskContext, CancellationToken, Task<Outputs>> run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }

    public DeploymentEventBuilder Task(string id, Action<TaskContext> run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }

    public DeploymentEventBuilder Task(string id, Action run, Action<TaskBuilder>? configure = null)
    {
        var task = new DelegateTask(id, run);
        this.eventTasks[id] = task;
        var builder = new TaskBuilder(task);
        configure?.Invoke(builder);
        return this;
    }
}