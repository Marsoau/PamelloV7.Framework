using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;

namespace PamelloV7.Framework.SampleApp;

class Program
{
    static async Task Main(string[] args) {
        var app = PamelloApp.CreateBuilder(args, new PamelloAppOptions() {
            UseApi = false,
        }).Build();
        
        await app.RunAsync();
    }
}
