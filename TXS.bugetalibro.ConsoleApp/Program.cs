using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.ConsoleApp.Infrastructure;
using TXS.bugetalibro.Infrastructure;
using System.IO;

namespace TXS.bugetalibro.ConsoleApp
{
    internal class Program
    {
        // https://wakeupandcode.com/generic-host-builder-in-asp-net-core-3-1/
        private static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.Services.GetService<IDataStoreInitializer>().MigrateAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((context, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile(
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                            optional: true);
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);

                    //if (context.HostingEnvironment.IsDevelopment())
                    {
                        configApp.AddUserSecrets("e04e704d-55fb-4c9a-bbd4-23fd3cac97f5");
                    }
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
                .UseConsoleLifetime();
        }
    }
}
