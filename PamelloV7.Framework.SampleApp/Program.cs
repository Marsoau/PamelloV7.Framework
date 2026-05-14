using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.SampleApp.Repositories;
using PamelloV7.Framework.SampleApp.Services;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.SampleApp;

[ConfigRoot]
public class TestNode
{
    public int MyConfigValue { get; set; } = 123;
    public ushort Port { get; set; } = 51630;
}

class Program
{
    static async Task Main(string[] args) {
        var app = PamelloApp.CreateBuilder(args, options => {
            options.ApiUrls.Add($"http://localhost:{TestConfig.Root.Port}");
        }).Build();
        
        await app.StartAsync();

        var items = app.Services.GetRequiredService<IItemRepository>();
        
        var database = app.Services.GetRequiredService<IDatabaseAccessService>();
        var collection = database.GetCollection<Item.Dbo>("testitems");

        items.Add(13, "test");
        items.Add(26, "another test");
        
        var anotherItems = app.Services.GetRequiredService<IPamelloRepository<Item>>();

        foreach (var item in anotherItems.GetAll()) {
            Console.WriteLine($"| {item}");
        }

        await app.WaitForShutdownAsync();
    }
}
