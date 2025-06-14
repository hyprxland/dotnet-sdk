using Hyprx.Dev.Messaging;

namespace Hyprx.Dev.Execution;

public class BusContext : RunContext
{
    public BusContext(RunContext context)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        this.Bus = context.GetRequiredService<IMessageBus>();
    }

    public IMessageBus Bus { get; }
}