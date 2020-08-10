using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.ConsoleApp.Commands;
using TXS.bugetalibro.ConsoleApp.Infrastructure;

namespace TXS.bugetalibro.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) => { }) // Add appsettings.json
                .ConfigureLogging((context, loggingBuilder) => { }) // Add logging
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging()
                        .AddOptions()
                        .AddSingleton<CommandLauncher>()
                        .AddSingleton<IHostedService, CliService>();

                    services.AddApplicationServices();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
