using PamelloV7.Framework.Shared.Exceptions;

namespace PamelloV7.Framework.Core.Exceptions;

public class PamelloDatabaseException : PamelloException
{
    public PamelloDatabaseException(string? message) : base(message) { }
}
