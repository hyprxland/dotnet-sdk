namespace Hyprx.Lodi;

public interface IServiceProviderLifetime : IDisposable
{
    IServiceProvider ServiceProvider { get; }
}