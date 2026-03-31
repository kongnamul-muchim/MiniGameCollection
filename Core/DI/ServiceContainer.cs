namespace Core.DI;

public class ServiceContainer : IServiceContainer
{
    private readonly Dictionary<Type, Func<object>> _registrations = new();
    private readonly Dictionary<Type, object> _singletons = new();
    private readonly object _lock = new();

    public void RegisterSingleton<TInterface, TImplementation>() 
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = () =>
        {
            lock (_lock)
            {
                if (!_singletons.TryGetValue(typeof(TInterface), out var instance))
                {
                    instance = Activator.CreateInstance<TImplementation>();
                    _singletons[typeof(TInterface)] = instance;
                }
                return instance;
            }
        };
    }

    public void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = () => 
            Activator.CreateInstance<TImplementation>();
    }

    public TInterface Resolve<TInterface>()
    {
        if (!_registrations.TryGetValue(typeof(TInterface), out var factory))
            throw new InvalidOperationException($"Service {typeof(TInterface).Name} is not registered");
        return (TInterface)factory();
    }
}