using System.Threading.Tasks;
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
        // https://wakeupandcode.com/generic-host-builder-in-asp-net-core-3-1/
        private static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<IDataStoreInitializer>().MigrateAsync();
            }
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, loggingBuilder) => { }) // Add logging
            .ConfigureServices((context, services) =>
            {
                services.AddLogging()
                    .AddOptions()
                    .AddTransient<CommandLauncher>()
                    .AddHostedService<CliService>()
                    .AddApplicationServices()
                    .AddInfrastructureServices();
            })
            .UseConsoleLifetime();
    }
}
