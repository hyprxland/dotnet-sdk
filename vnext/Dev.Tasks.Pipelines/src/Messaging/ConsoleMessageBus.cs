using System.Collections.Concurrent;
using System.Diagnostics;

namespace Hyprx.Dev.Messaging;

public sealed class ConsoleMessageBus : IMessageBus
{
    private readonly Thread thread;

    private readonly AutoResetEvent runner = new AutoResetEvent(false);

    private readonly ConcurrentQueue<IMessage> messages = new ConcurrentQueue<IMessage>();

    private readonly ConcurrentBag<Subscription> subscriptions = new ConcurrentBag<Subscription>();

    private bool shuttingDown = false;

    private bool disposed = false;

    private DiagnosticLevel minLevel = DiagnosticLevel.Info;

    public ConsoleMessageBus()
    {
        this.thread = new Thread(this.PumpMessages);
        this.thread.Start();
    }

    public bool IsListening => !this.shuttingDown && !this.disposed;

    public void Dispose()
    {
        if (this.disposed)
            return;

        this.disposed = true;
        GC.SuppressFinalize(this);
        this.shuttingDown = true;
        this.runner.Set();
        this.thread.Join();
        this.runner.Dispose();
    }

#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates

    public async void Broadcast()
    {
        try
        {
            while (this.messages.TryDequeue(out var message))
            {
                var clone = this.subscriptions.ToArray();
                foreach (var sub in clone)
                {
                    if (sub.WantsTopic(message.Topic))
                    {
                        try
                        {
                            var completed = await sub.Subscriber.ReceiveAsync(message);
                            if (completed)
                                sub.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex, "Exceptions:Unhandled");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex, "Exceptions:Unhandled");
        }
    }

    public bool IsEnabled(DiagnosticLevel level)
    => level >= this.minLevel;

    public void Send(IMessage message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if (this.shuttingDown || this.disposed)
            throw new ObjectDisposedException(nameof(ConsoleMessageBus));

        if (message is DiagnosticMessage dm && dm.Level < this.minLevel)
            return;

        this.messages.Enqueue(message);
        this.runner.Set();
    }

    public void SetLevel(DiagnosticLevel level)
        => this.minLevel = level;

    internal void Unsubscribe(Subscription? subscription)
    {
        if (subscription == null)
            return;

        this.subscriptions.TryTake(out subscription);
    }

    private void PumpMessages()
    {
        while (!this.shuttingDown)
        {
            this.runner.WaitOne();
            this.Broadcast();
        }

        this.Broadcast();
    }
}

internal sealed class Subscription : IDisposable
{
    private readonly ConsoleMessageBus bus;

    public Subscription(ConsoleMessageBus bus, IMessageSubscriber subscriber, params string[] topics)
    {
        this.Subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
        this.Topics = topics ?? throw new ArgumentNullException(nameof(topics));
        this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public IMessageSubscriber Subscriber { get; }

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