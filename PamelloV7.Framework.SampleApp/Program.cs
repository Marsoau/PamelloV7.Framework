using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Actions;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Config.Attributes;
using PamelloV7.Framework.Core.Context;
using PamelloV7.Framework.Core.Data;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Core.PEQL;
using PamelloV7.Framework.Core.Repositories;
using PamelloV7.Framework.Core.Scope;
using PamelloV7.Framework.PEQL.Filters;
using PamelloV7.Framework.PEQL.Operators;
using PamelloV7.Framework.SampleApp.Entities;
using PamelloV7.Framework.SampleApp.Repositories;
using PamelloV7.Framework.SampleApp.Scope;
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
        
        var peql = app.Services.GetRequiredService<IPamelloEntityQueryService>();
        
        Console.WriteLine("Items:");
        
        await foreach (var item in peql.GetAsync<Item>("(1,2,3*3,2,1)#it#unique")) {
            Console.WriteLine(item);
        }

        await app.StopAsync();
    }
}
