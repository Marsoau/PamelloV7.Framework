using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PamelloV7.Framework.App;
using PamelloV7.Framework.Core.Config;
using PamelloV7.Framework.Core.Logging;
using PamelloV7.Framework.Shared.Variants.Attributes;

namespace PamelloV7.Framework.SampleApp;

partial class TC
{
    private static int NumberFromString(string str) => int.Parse(str);
    private static int NumberFromDateTime(DateTime date) => date.DayOfYear;
    
    public void Method(
        [Variant(nameof(NumberFromString))]
        [Variant(nameof(NumberFromDateTime))]
        int number
    ) {
        Console.WriteLine($"Number is: {number}");
    }
}

class Program
{
    static async Task Main(string[] args) {
        var app = PamelloApp.CreateBuilder(args, new PamelloAppOptions() {
            UseApi = true,
        }).Build();
        
        await app.StartAsync();
        
        await Test();

        await app.WaitForShutdownAsync();
    }

    static async Task Test() {
        PamelloOutput.Write(ServerConfig.Root.AllowUserCreation);
        
        var tc = new TC();
        
        tc.Method(3);
        tc.Method("123");
        tc.Method(DateTime.Now);
    }
}
