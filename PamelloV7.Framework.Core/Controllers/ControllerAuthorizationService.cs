using PamelloV7.Framework.Core.Services.Base;
using PamelloV7.Framework.Shared.Entities;
using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.Controllers;

public class ControllerAuthorizationService : IPamelloService
{
    public virtual IPamelloBasicUser? GetUser() {
        throw new PamelloException(
            """
            User authorization is not implemented yet
            Inherit from ControllerAuthorizationService and override GetUser method
            
            Also by default some default controllers might require user, and if you don't want to use user in your controller, you can override IsUserRequired property to false
            """
        );
    }
}
