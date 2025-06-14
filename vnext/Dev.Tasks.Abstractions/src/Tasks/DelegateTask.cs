using Hyprx.Dev.Collections;
using Hyprx.Results;

namespace Hyprx.Dev.Tasks;

public class DelegateTask : CodeTask, ITaskHandler
{
    private readonly RunTaskAsync func;

    public DelegateTask(string id, RunTaskAsync taskFunc)
        : base(id)
    {
        this.func = taskFunc ?? throw new ArgumentNullException(nameof(taskFunc));
    }

    public DelegateTask(string id, Func<TaskContext, Task<Result<Outputs>>> func)
        : base(id)
    {
        this.func = async (context, cancellationToken) => await func(context);
    }

    public DelegateTask(string id, Func<TaskContext, CancellationToken, Outputs> func)
        : base(id)
    {
        this.func = (context, cancellationToken) =>
        {
            try
            {
                var outputs = func(context, cancellationToken);
                var result = new Result<Outputs>(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
    }

    public DelegateTask(string id, Func<TaskContext, CancellationToken, Task<Outputs>> func)
        : base(id)
    {
        this.func = async (context, cancellationToken) =>
        {
            try
            {
                var outputs = await func(context, cancellationToken);
                var result = new Result<Outputs>(outputs);
                return result;
            }
            catch (Exception ex)
            {
                var result = new Result<Outputs>(ex);
                return result;
            }
        };
    }

    public DelegateTask(string id, Func<TaskContext, Outputs> func)
        : base(id)
    {
        this.func = (context, cancellationToken) =>
        {
            try
            {
                var outputs = func(context);
                var result = new Result<Outputs>(outputs);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
    }

    public DelegateTask(string id, Action<TaskContext> func)
        : base(id)
    {
        this.func = (context, cancellationToken) =>
        {
            try
            {
                func(context);
                var result = new Result<Outputs>(new Outputs());
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result<Outputs>(ex);
                return Task.FromResult(result);
            }
        };
    }

    public DelegateTask(string id, Action func)
        : base(id)
    {
        this.func = (context, cancellationToken) =>
        {
            try
            {
                func();
                var result = new Result<Outputs>(new Outputs());
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new Result<Outputs>(ex);
                return result;
            }
        };
    }

    public virtual async Task<Result<Outputs>> RunAsync(TaskContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await this.func(context, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}