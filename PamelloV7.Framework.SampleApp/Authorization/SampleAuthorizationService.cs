using Microsoft.AspNetCore.Http;
using PamelloV7.Framework.Core.Controllers;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.Shared.Entities;

namespace PamelloV7.Framework.SampleApp.Authorization;

public class SampleAuthorizationService : ControllerAuthorizationService
{
    public override IPamelloBasicUser? GetUser(HttpRequest request) {
        return new User("Gay");
    }
}
