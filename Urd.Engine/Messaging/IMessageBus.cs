namespace Urd.Engine.Messaging;

/// <summary>
///     Pub/sub message bus. Publishers and subscribers are decoupled; message types are defined by callers.
/// </summary>
public interface IMessageBus
{
    /// <summary>Publish <paramref name="message" /> to all current subscribers of <typeparamref name="TMessage" />.</summary>
    void Publish<TMessage>(TMessage message);

    /// <summary>
    ///     Subscribe to messages of type <typeparamref name="TMessage" />.
    ///     Returns a token; dispose it to unsubscribe.
    /// </summary>
    IDisposable Subscribe<TMessage>(Action<TMessage> handler);
}