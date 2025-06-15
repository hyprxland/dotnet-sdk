namespace Hyprx.Rex.Execution;

public class Deferred<T, TContext, TValue>
    where T : Deferred<T, TContext, TValue>
    where TContext : RunContext
{
    private Func<TContext, CancellationToken, Task<TValue>> taskFunc;

    private bool isCompleted = false;

    private TValue? result;

    public Deferred()
    {
        this.result = default;
        this.isCompleted = true;
        this.taskFunc = (context, cancellationToken) =>
        {
            return Task.FromResult(this.result!);
        };
    }

    public Deferred(TValue result)
    {
        this.result = result;
        this.isCompleted = true;

        this.taskFunc = (context, cancellationToken) =>
        {
            return Task.FromResult(result);
        };
    }

    public Deferred(Func<TContext, TValue> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        this.taskFunc = (context, cancellationToken) =>
        {
            return Task.FromResult(func(context));
        };
    }

    public Deferred(Func<TValue> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        this.taskFunc = (context, cancellationToken) =>
        {
            var result = func();
            this.result = result;
            return Task.FromResult(result);
        };
    }

    public Deferred(Func<TContext, CancellationToken, Task<TValue>> taskFunc)
    {
        this.taskFunc = taskFunc ?? throw new ArgumentNullException(nameof(taskFunc));
    }

    public TValue Value
    {
        get
        {
            if (!this.isCompleted)
            {
                throw new InvalidOperationException("Task has not been completed yet.");
            }

            return this.result!;
        }
    }

    public bool HasValue => this.isCompleted;

    public T Defer(TValue value)
    {
        this.result = value;
        this.isCompleted = true;

        this.taskFunc = (context, cancellationToken) =>
        {
            return Task.FromResult(value);
        };

        return (T)this;
    }

    public T Defer(Func<TValue> valueFactory)
    {
        this.result = valueFactory();
        this.isCompleted = true;

        this.taskFunc = (context, cancellationToken) =>
        {
            return Task.FromResult(this.result!);
        };

        return (T)this;
    }

    public T Defer(Func<TContext, TValue> valueFactory)
    {
        if (valueFactory == null)
        {
            throw new ArgumentNullException(nameof(valueFactory));
        }

        this.taskFunc = (context, cancellationToken) =>
        {
            var result = valueFactory(context);
            this.result = result;
            return Task.FromResult(result);
        };

        return (T)this;
    }

    public T Defer(Func<TContext, CancellationToken, Task<TValue>> taskFunc)
    {
        this.taskFunc = taskFunc ?? throw new ArgumentNullException(nameof(taskFunc));
        this.isCompleted = false;
        return (T)this;
    }

    public async Task<TValue> ResolveAsync(TContext context, CancellationToken cancellationToken = default)
    {
        if (this.isCompleted)
        {
            return this.result!;
        }

        var result = await this.taskFunc(context, cancellationToken);
        this.result = result;
        this.isCompleted = true;
        return result;
    }
}