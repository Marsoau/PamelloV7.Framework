using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.SampleApp.Services;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.SampleApp;

[ConfigRoot]
public class TestNode
{
    public int MyConfigValue { get; set; } = 123;
    public ushort Port { get; set; } = 51630;
}

public class DatabaseItem
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
}

class Program
{
    static async Task Main(string[] args) {
        var app = PamelloApp.CreateBuilder(args, options => {
            options.ApiUrls.Add($"http://localhost:{TestConfig.Root.Port}");
        }).Build();
        
        await app.StartAsync();
        
        var database = app.Services.GetRequiredService<IDatabaseAccessService>();
        var collection = database.GetCollection<DatabaseItem>("test");

        foreach (var item in collection.GetAll()) {
            Console.WriteLine(item.Message);
        }

        await app.WaitForShutdownAsync();
    }
}
