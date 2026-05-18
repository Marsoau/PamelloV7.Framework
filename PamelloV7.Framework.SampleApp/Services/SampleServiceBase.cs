using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.SampleApp.Services;

public interface ISampleService : IPamelloService
{
    string GetMessage();
}

public abstract class SampleServiceBase : ISampleService
{
    public virtual string GetMessage() => "Sample Service Message";
}

public class SampleService : SampleServiceBase
{
    public override string GetMessage() => "Overridden Sample Service Message";
}
