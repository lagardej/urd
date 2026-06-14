using Urd.Engine.Component;
using Urd.Engine.Messaging;

namespace Urd.Tests;

public record DummyParameters(string Greeting);

public sealed class DummyComponent : BaseComponent
{
    public DummyComponent(IMessageBus bus) : base(bus)
    {
        bus.Subscribe<ParametersChanged<DummyParameters>>(OnParametersChanged);
        ScheduleAt(0);
    }

    protected override ulong Cadence => 10;

    public string CurrentGreeting { get; private set; } = "Hello";

    private void OnParametersChanged(ParametersChanged<DummyParameters> evt)
    {
        CurrentGreeting = evt.Parameters.Greeting;
    }

    protected override void Tick(ulong elapsedSeconds)
    {
    }
}