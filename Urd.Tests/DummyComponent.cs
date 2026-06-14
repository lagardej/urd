using UnitsNet;
using UnitsNet.Units;
using Urd.Engine.Component;
using Urd.Engine.Data.Normalization;
using Urd.Engine.Data.Validation;
using Urd.Engine.Messaging;

namespace Urd.Tests;

public record DummyParameters(
    string Greeting,
    [property: WrapAround(0, 360, AngleUnit.Degree)]
    [property: QuantityRange(0, 360, AngleUnit.Degree)]
    Angle Phase
);

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

    protected override void Tick(TickContext context)
    {
    }
}