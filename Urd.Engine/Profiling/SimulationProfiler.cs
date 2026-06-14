using System.Collections.Concurrent;
using Urd.Engine.Component;
using Urd.Engine.Messaging;

namespace Urd.Engine.Profiling;

/// <summary>
///     Headless profiler. Subscribes to <see cref="ComponentTicked" /> and accumulates
///     per-component statistics. Data is exposed via <see cref="Profiles" /> for consumers
///     such as the rendering layer.
/// </summary>
public sealed class Profiler
{
    public const int DefaultWindowSize = 1000;
    private readonly ConcurrentDictionary<string, ComponentProfile> _profiles = new();

    private readonly int _windowSize;

    public Profiler(IMessageBus bus, int windowSize = DefaultWindowSize)
    {
        _windowSize = windowSize;
        bus.Subscribe<ComponentTicked>(OnComponentTicked);
    }

    /// <summary>Per-component profiles, keyed by component ID.</summary>
    public IReadOnlyDictionary<string, ComponentProfile> Profiles => _profiles;

    private void OnComponentTicked(ComponentTicked evt)
    {
        var profile = _profiles.GetOrAdd(evt.ComponentId, _ => new ComponentProfile(_windowSize));
        profile.Record(evt.Duration);
    }
}
