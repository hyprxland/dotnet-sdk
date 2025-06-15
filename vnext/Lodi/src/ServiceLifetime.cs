namespace Hyprx.Lodi;

public struct ServiceLifetime
{
    public ServiceLifetime(string name)
    {
        this.Name = name;
    }

    public ServiceLifetime()
    {
        this.Name = "Transient";
    }

    public static ServiceLifetime Transient { get; } = new("Transient");

    public static ServiceLifetime Scoped { get; } = new("Scoped");

    public static ServiceLifetime Singleton { get; } = new("Singleton");

    public string Name { get; init; } = null!;

    public static implicit operator ServiceLifetime(string name)
    {
        return new ServiceLifetime(name);
    }

    public static implicit operator string(ServiceLifetime lifetime)
    {
        return lifetime.Name;
    }

    public static bool IsTransient(ServiceLifetime lifetime)
    {
        return lifetime.Name == Transient.Name;
    }

    public static bool IsScoped(ServiceLifetime lifetime)
    {
        return lifetime.Name == Scoped.Name;
    }

    public static bool IsSingleton(ServiceLifetime lifetime)
    {
        return lifetime.Name == Singleton.Name;
    }

    public override string ToString()
    {
        return this.Name;
    }
}