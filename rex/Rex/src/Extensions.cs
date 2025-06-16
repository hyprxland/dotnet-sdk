using System.CommandLine;
using System.CommandLine.Invocation;

namespace Rex;

public static class Extensions
{
    public static Command WithHandler(this Command command, Func<int> handler)
    {
        command.SetHandler((ctx) =>
        {
            var result = handler();
            ctx.ExitCode = result;
        });
        return command;
    }

    public static Command WithHandler(this Command command, Func<InvocationContext, int> handler)
    {
        command.SetHandler((ctx) =>
        {
            var result = handler(ctx);
            ctx.ExitCode = result;
        });
        return command;
    }

    public static Command WithHandler(this Command command, Func<Task<int>> handler)
    {
        command.SetHandler(async (ctx) =>
        {
            var result = await handler();
            ctx.ExitCode = result;
        });
        return command;
    }

    public static Command WithHandler(this Command command, Func<InvocationContext, Task<int>> handler)
    {
        command.SetHandler(async (ctx) =>
        {
            var result = await handler(ctx);
            ctx.ExitCode = result;
        });
        return command;
    }
}