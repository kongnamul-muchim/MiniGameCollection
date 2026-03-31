using Core.DI;
using Xunit;

namespace Core.Tests;

public class DITests
{
    public interface ITestService
    {
        int GetValue();
    }

    public class TestService : ITestService
    {
        public int GetValue() => 42;
    }

    public interface IAnotherService
    {
        string GetName();
    }

    public class AnotherService : IAnotherService
    {
        public string GetName() => "Test";
    }

    [Fact]
    public void Container_ShouldRegisterSingleton()
    {
        var container = new ServiceContainer();
        container.RegisterSingleton<ITestService, TestService>();

        var instance1 = container.Resolve<ITestService>();
        var instance2 = container.Resolve<ITestService>();

        Assert.Same(instance1, instance2);
        Assert.Equal(42, instance1.GetValue());
    }

    [Fact]
    public void Container_ShouldRegisterTransient()
    {
        var container = new ServiceContainer();
        container.RegisterTransient<ITestService, TestService>();

        var instance1 = container.Resolve<ITestService>();
        var instance2 = container.Resolve<ITestService>();

        Assert.NotSame(instance1, instance2);
        Assert.Equal(42, instance1.GetValue());
        Assert.Equal(42, instance2.GetValue());
    }

    [Fact]
    public void Container_ShouldThrowWhenNotRegistered()
    {
        var container = new ServiceContainer();

        Assert.Throws<InvalidOperationException>(() => container.Resolve<ITestService>());
    }

    [Fact]
    public void Container_ShouldRegisterMultipleServices()
    {
        var container = new ServiceContainer();
        container.RegisterSingleton<ITestService, TestService>();
        container.RegisterTransient<IAnotherService, AnotherService>();

        var testService = container.Resolve<ITestService>();
        var anotherService = container.Resolve<IAnotherService>();

        Assert.Equal(42, testService.GetValue());
        Assert.Equal("Test", anotherService.GetName());
    }
}