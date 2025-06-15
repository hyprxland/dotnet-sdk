using Hyprx.Dev.Messaging;

namespace Hyprx.Dev.Execution;

public class BusContext : RunContext
{
    public BusContext(RunContext context)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        var bus = context.Services.GetService(typeof(IMessageBus)) as IMessageBus;
        if (bus == null)
            throw new InvalidOperationException("No message bus registered in the service provider.");
        this.Bus = bus;
    }

    public IMessageBus Bus { get; }
}