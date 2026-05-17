using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.SampleApp.Repositories;
using PamelloV7.Framework.SampleApp.Services;
using PamelloV7.Framework.Shared.Attributes;
using PamelloV7.Framework.Shared.Entities.Containers;
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
        var app = PamelloApp.CreateBuilder(
            args, new PamelloAppOptions() {
                Logger = new PamelloConsoleLogger(),
            }, options => {
                options.ApiUrls.Add($"http://localhost:{TestConfig.Root.Port}");
            }
        ).Build();
        await app.StartAsync();

        var items = app.Services.GetRequiredService<ItemRepository>();
        var queries = app.Services.GetRequiredService<IPamelloEntityQueryService>();
        
        //
        //
        //

        var container = new Safe<Item>(1);

        var user1 = new User("Gera");
        var user2 = new User("Pivo");
        
        PamelloAppScope.SetUserIn(user1, () => {
            Console.WriteLine($"User: {PamelloAppScope.User}");
            Console.WriteLine($"Entity: {container.Entity}");
        });
        PamelloAppScope.SetUserIn(user2, () => {
            Console.WriteLine($"User: {PamelloAppScope.User}");
            Console.WriteLine($"Entity: {container.Entity}");
        });
        
        //
        //
        //
        
        var allItems = items.GetAll().ToList();
        
        Console.WriteLine($"Items: {allItems.Count}");
        foreach (var item in allItems) {
            Console.WriteLine($"| {item}");
        }

        await app.WaitForShutdownAsync();
    }
}
