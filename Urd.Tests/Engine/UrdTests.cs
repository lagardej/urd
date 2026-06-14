using UnitsNet;
using Urd.Engine.Component;
using Urd.Engine.Scheduling;
using Xunit;

namespace Urd.Tests.Engine;

public sealed class UrdTests
{
    private readonly InMemoryMessageBus _bus = new();
    private static DummyParameters DefaultParameters => new("Hello", Angle.FromDegrees(180));

    [Fact]
    public void ParameterChange_IsReflectedInComponent()
    {
        var engine = new Urd.Engine.Urd(_bus);
        _ = new Scheduler(_bus);
        var component = new DummyComponent(_bus);

        _bus.Publish(new ParametersChanged<DummyParameters>(DefaultParameters with { Greeting = "Hi" }));
        engine.Tick();

        Assert.Equal("Hi", component.CurrentGreeting);
    }
}
