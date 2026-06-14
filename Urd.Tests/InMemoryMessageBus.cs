using Urd.Engine.Messaging;

namespace Urd.Tests;

public sealed class InMemoryMessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Publish<TMessage>(TMessage message)
    {
        if (!_subscribers.TryGetValue(typeof(TMessage), out var handlers)) return;
        foreach (var handler in handlers)
            ((Action<TMessage>)handler)(message!);
    }

    public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
    {
        if (!_subscribers.TryGetValue(typeof(TMessage), out var handlers))
            _subscribers[typeof(TMessage)] = handlers = new List<Delegate>();
        handlers.Add(handler);
        return new Subscription(() => handlers.Remove(handler));
    }

    private sealed class Subscription(Action unsubscribe) : IDisposable
    {
        public void Dispose()
        {
            unsubscribe();
        }
    }
}