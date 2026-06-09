using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace PamelloV7.Framework.Core.Controllers;

public abstract class PamelloControllerBase : ControllerBase
{
    protected readonly IServiceProvider Services;
    
    protected ControllerAuthorizationService Authorization => field ??= Services.GetRequiredService<ControllerAuthorizationService>();
    
    public PamelloControllerBase(IServiceProvider services) {
        Services = services;
    }
}
