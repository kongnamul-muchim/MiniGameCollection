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
                    instance = CreateInstance(typeof(TImplementation));
                    _singletons[typeof(TInterface)] = instance;
                }
                return instance;
            }
        };
    }

    public void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = () => CreateInstance(typeof(TImplementation));
    }

    public void RegisterSingleton<TInterface>(TInterface instance)
    {
        _registrations[typeof(TInterface)] = () => instance;
        _singletons[typeof(TInterface)] = instance!;
    }

    public void RegisterSingleton<TInterface>(Func<TInterface> factory)
    {
        _registrations[typeof(TInterface)] = () =>
        {
            lock (_lock)
            {
                if (!_singletons.TryGetValue(typeof(TInterface), out var instance))
                {
                    instance = factory()!;
                    _singletons[typeof(TInterface)] = instance;
                }
                return instance;
            }
        };
    }

    public void RegisterTransient<TInterface>(Func<TInterface> factory)
    {
        _registrations[typeof(TInterface)] = () => factory()!;
    }

    public TInterface Resolve<TInterface>()
    {
        if (!_registrations.TryGetValue(typeof(TInterface), out var factory))
            throw new InvalidOperationException($"Service {typeof(TInterface).Name} is not registered");
        return (TInterface)factory();
    }

    public bool IsRegistered<TInterface>()
    {
        return _registrations.ContainsKey(typeof(TInterface));
    }

    private object CreateInstance(Type type)
    {
        var ctor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        if (ctor == null)
            return Activator.CreateInstance(type)!;

        var parameters = ctor.GetParameters();
        var args = new object[parameters.Length];
        
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            if (_registrations.TryGetValue(paramType, out var factory))
            {
                args[i] = factory();
            }
            else
            {
                // Try to create the dependency recursively
                args[i] = CreateInstance(paramType);
            }
        }
        
        return ctor.Invoke(args);
    }
}
