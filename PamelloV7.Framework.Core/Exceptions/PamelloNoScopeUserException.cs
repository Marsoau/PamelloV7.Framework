using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.Exceptions;

public class PamelloNoScopeUserException : PamelloException
{
    public PamelloNoScopeUserException(string? message = "No scope user") : base(message) { }
}
