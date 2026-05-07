using PamelloV7.Framework.Core.Services.Base;

namespace PamelloV7.Framework.SampleApp.Services;

public interface ISampleService : IPamelloService
{
    string GetMessage();
}

public class SampleService : ISampleService
{
    public string GetMessage() => "Sample Service Message";
}
