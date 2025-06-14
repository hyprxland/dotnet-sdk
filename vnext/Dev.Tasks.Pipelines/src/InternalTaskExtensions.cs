using Hyprx.Dev.Execution;
using Hyprx.Dev.Messaging;

namespace Hyprx.Dev;

internal static class InternalTaskExtensions
{
    public static T? GetService<T>(this RunContext context)
        where T : class
    {
        var service = context.Services.GetService(typeof(T)) as T;
        return service;
    }

    public static T GetRequiredService<T>(this RunContext context)
        where T : class
    {
        var service = context.Services.GetService(typeof(T)) as T;
        if (service == null)
        {
            throw new InvalidOperationException($"Required service of type {typeof(T).FullName} is not available.");
        }

        return service;
    }
}