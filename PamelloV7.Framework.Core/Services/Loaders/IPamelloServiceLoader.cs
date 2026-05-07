using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Framework.Core.Services.Loaders;

public interface IPamelloServiceLoader
{
    public void LoadAppServices();
    public void ConfigureAppServices(IServiceCollection collection);
    public void StartAppServices();
}
