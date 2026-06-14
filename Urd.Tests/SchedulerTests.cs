using Urd.Engine.Component;
using Xunit;

namespace Urd.Tests;

public sealed class UrdTests
{
    private readonly InMemoryMessageBus _bus = new();

    [Fact]
    public void Engine_ParameterChange_IsReflectedInComponent()
    {
        var engine = new Engine.Urd(_bus);
        _ = new Engine.Scheduling.Urd(_bus);
        var component = new DummyComponent(_bus);
        var parameters = new DummyParameters("Hi");
        var parametersChanged = new ParametersChanged<DummyParameters>(parameters);

        _bus.Publish(parametersChanged);
        engine.Tick();

        Assert.Equal("Hi", component.CurrentGreeting);
    }
}
