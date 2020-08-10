using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.ConsoleApp.Infrastructure;
using TXS.bugetalibro.Infrastructure;

namespace TXS.bugetalibro.ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                }) 
                .ConfigureLogging((context, loggingBuilder) => { }) // Add logging
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging()
                        .AddOptions()
                        .AddSingleton<CommandLauncher>()
                        .AddSingleton<IHostedService, CliService>();

                    services.AddApplicationServices()
                        .AddInfrastructureServices();
                })
                .UseConsoleLifetime()
                .Build();

            await host.Services.GetService<IDataStoreInitializer>().MigrateAsync();
            await host.RunAsync();
        }
    }
}
