namespace Core.DI;

public interface IServiceContainer
{
    void RegisterSingleton<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    void RegisterTransient<TInterface, TImplementation>() 
        where TImplementation : class, TInterface;
    TInterface Resolve<TInterface>();
}