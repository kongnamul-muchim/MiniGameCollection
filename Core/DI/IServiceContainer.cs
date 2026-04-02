namespace Core.DI;

public interface IServiceContainer
{
    void RegisterSingleton<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    void RegisterSingleton<TInterface>(TInterface instance);
    void RegisterSingleton<TInterface>(Func<TInterface> factory);
    void RegisterTransient<TInterface>(Func<TInterface> factory);
    TInterface Resolve<TInterface>();
    bool IsRegistered<TInterface>();
}
