using System.Collections.Concurrent;
using System.Diagnostics;

namespace Hyprx.Rex.Messaging;

/*
public sealed class ConsoleMessageQueueBus : IMessageBus
{
    private static readonly Lazy<ConsoleMessageQueueBus> instance = new Lazy<ConsoleMessageQueueBus>(() => new ConsoleMessageQueueBus());

    private readonly Thread thread;

    private readonly AutoResetEvent runner = new AutoResetEvent(false);

    private readonly ConcurrentQueue<IMessage> messages = new ConcurrentQueue<IMessage>();

    private readonly ConcurrentBag<Subscription> subscriptions = new ConcurrentBag<Subscription>();

    private bool shuttingDown = false;

    private bool disposed = false;

    private DiagnosticLevel minLevel = DiagnosticLevel.Info;

    public ConsoleMessageQueueBus()
    {
        this.thread = new Thread(this.PumpMessages);
        this.thread.Start();
    }

    public static ConsoleMessageQueueBus Default => instance.Value;

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

    public async Task Broadcast()
    {
        try
        {
            while (this.messages.TryDequeue(out var message))
            {
                var clone = this.subscriptions.ToArray();
                Console.WriteLine("clone subs: " + clone.Length);
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
            throw new ObjectDisposedException(nameof(ConsoleMessageQueueBus));

        if (message is DiagnosticMessage dm && dm.Level < this.minLevel)
            return;

        Console.WriteLine($"enqueue: {message.Topic}");

        this.messages.Enqueue(message);
        this.runner.Set();
    }

    public void SetLevel(DiagnosticLevel level)
        => this.minLevel = level;

    public IDisposable Subscribe(IMessageSink subscriber, params string[] topics)
    {
        ArgumentNullException.ThrowIfNull(subscriber, nameof(subscriber));
        ArgumentNullException.ThrowIfNull(topics, nameof(topics));

        if (this.shuttingDown || this.disposed)
            throw new ObjectDisposedException(nameof(ConsoleMessageQueueBus));

        var sub = new Subscription(this, subscriber, topics);
        this.subscriptions.Add(sub);
        Console.WriteLine("subscribed: " + sub);
        return sub;
    }

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
    private readonly ConsoleMessageQueueBus bus;

    public Subscription(ConsoleMessageQueueBus bus, IMessageSink subscriber, params string[] topics)
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
*/