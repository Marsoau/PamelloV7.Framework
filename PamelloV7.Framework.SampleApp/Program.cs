using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Logging;

namespace PamelloV7.Framework.SampleApp;

class Program
{
    static async Task Main(string[] args) {
        var app = PamelloApp.CreateBuilder(args, new PamelloAppOptions() {
            UseApi = true,
        }).Build();
        
        await app.StartAsync();
        
        PamelloOutput.Write("Hello World!");

        await app.WaitForShutdownAsync();
    }
}
