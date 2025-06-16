using System.Collections.Concurrent;

namespace Hyprx.Rex.Messaging;

public class ConsoleMessageBus : IMessageBus
{
    private readonly ConcurrentBag<Subscription> subscriptions = new ConcurrentBag<Subscription>();

    private DiagnosticLevel minLevel = DiagnosticLevel.Info;

    public void SetMinimumLevel(DiagnosticLevel level)
    {
        this.minLevel = level;
    }

    public void Send(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        if (message is DiagnosticMessage dm && dm.Level < this.minLevel)
        {
            return;
        }

        var clone = this.subscriptions.ToArray();
        foreach (var sub in clone)
        {
            if (sub.WantsTopic(message.Topic))
            {
                _ = sub.Subscriber.ReceiveAsync(message);
            }
        }
    }

    public Task SendAsync(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        var clone = this.subscriptions.ToArray();
        var tasks = new List<Task<bool>>(clone.Length);

        if (message is DiagnosticMessage dm && dm.Level < this.minLevel)
        {
            return Task.CompletedTask;
        }

        foreach (var sub in clone)
        {
            if (sub.WantsTopic(message.Topic))
            {
                tasks.Add(sub.Subscriber.ReceiveAsync(message));
            }
        }

        return Task.WhenAll(tasks);
    }

    public IDisposable Subscribe(IMessageSink subscriber, params string[] topics)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));
        ArgumentNullException.ThrowIfNull(topics, nameof(topics));

        var sub = new Subscription(this, subscriber, topics);
        this.subscriptions.Add(sub);
        return sub;
    }

    internal void Unsubscribe(Subscription? subscription)
    {
        if (subscription == null)
            return;

        this.subscriptions.TryTake(out subscription);
    }

    internal sealed class Subscription : IDisposable
    {
        private readonly ConsoleMessageBus bus;

        public Subscription(ConsoleMessageBus bus, IMessageSink subscriber, params string[] topics)
        {
            this.Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
            this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public IMessageSink Subscriber { get; }

        public string[] Topics { get; }

        public bool WantsTopic(string topic)
        {
            foreach (var t in this.Topics)
            {
                if (t == "*" || t == topic || (t.EndsWith('*') && topic.StartsWith(t[..^1])))
                    return true;
            }

            return false;
        }

        public void Dispose()
        {
            this.bus.Unsubscribe(this);
        }
    }
}