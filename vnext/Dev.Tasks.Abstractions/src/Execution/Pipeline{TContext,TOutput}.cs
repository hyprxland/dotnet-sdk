namespace Hyprx.Dev.Execution;

public abstract class Pipeline<TContext, TOutput>
{
    private readonly List<IPipelineMiddleware<TContext>> middlewares = new();

    public Pipeline<TContext, TOutput> Use(IPipelineMiddleware<TContext> middleware)
    {
        this.middlewares.Add(middleware);
        return this;
    }

    public Pipeline<TContext, TOutput> Use(Func<TContext, Func<Task>, CancellationToken, Task> nextAsync)
    {
        if (nextAsync == null)
        {
            throw new ArgumentNullException(nameof(nextAsync));
        }

        return this.Use(new DelegatePipelineMiddleware<TContext>(nextAsync));
    }

    public abstract Task<TOutput> RunAsync(TContext context, CancellationToken cancellationToken = default);

    protected async Task<TContext> PipeAsync(TContext context, CancellationToken cancellationToken = default)
    {
        var prevIndex = -1;

        var ordered = this.middlewares.ToList();

        ordered.Reverse();

        async Task Run(int index, TContext ctx, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            if (index == prevIndex)
            {
                throw new InvalidOperationException("Middleware cannot be executed multiple times in a single pipeline run.");
            }

            if (index >= ordered.Count)
            {
                return;
            }

            prevIndex = index;

            var middleware = ordered[index];

            await middleware.NextAsync(ctx, async () => await Run(index + 1, ctx, token), token);
        }

        await Run(0, context, cancellationToken);
        return context;
    }
}

public interface IPipelineMiddleware<TContext>
{
    Task NextAsync(TContext context, Func<Task> next, CancellationToken cancellationToken = default);
}

public class DelegatePipelineMiddleware<TContext> : IPipelineMiddleware<TContext>
{
    private readonly Func<TContext, Func<Task>, CancellationToken, Task> nextAsync;

    public DelegatePipelineMiddleware(Func<TContext, Func<Task>, CancellationToken, Task> nextAsync)
    {
        this.nextAsync = nextAsync ?? throw new ArgumentNullException(nameof(nextAsync));
    }

    public Task NextAsync(TContext context, Func<Task> next, CancellationToken cancellationToken = default)
    {
        return this.nextAsync(context, next, cancellationToken);
    }
}