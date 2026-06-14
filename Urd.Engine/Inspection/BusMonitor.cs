using System.Collections.Concurrent;
using Urd.Engine.Messaging;

namespace Urd.Engine.Inspection;

/// <summary>
///     Wraps <see cref="IMessageBus" /> and counts publishes per message type.
///     Pass this as the bus to all engine components; consumers subscribe to the same instance.
/// </summary>
public sealed class BusMonitor : IMessageBus
{
    private readonly IMessageBus _inner;
    private readonly ConcurrentDictionary<string, long> _counts = new();

    public BusMonitor(IMessageBus inner)
    {
        _inner = inner;
    }

    /// <summary>Publish counts keyed by message type name.</summary>
    public IReadOnlyDictionary<string, long> Counts => _counts;

    public void Publish<TMessage>(TMessage message)
    {
        _counts.AddOrUpdate(typeof(TMessage).Name, 1, (_, n) => n + 1);
        _inner.Publish(message);
    }

    public IDisposable Subscribe<TMessage>(Action<TMessage> handler) =>
        _inner.Subscribe(handler);
}
