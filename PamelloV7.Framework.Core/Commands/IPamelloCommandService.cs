using PamelloV7.Framework.Core.Commands.Base;
using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.Core.Commands;

public interface IPamelloCommandService : IPamelloService
{
    public TCommand Get<TCommand>()
        where TCommand : PamelloCommand, new();
    public PamelloCommand Get(Type commandType);
    public Task<object?> ExecutePathAsync(string commandPath);
}
