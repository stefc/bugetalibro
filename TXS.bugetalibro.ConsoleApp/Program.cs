using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.ConsoleApp.Commands;

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
                    services.AddLogging();
                    services.AddOptions();
                    services.AddSingleton<IHostedService, ConsoleCommandService>();

                    services.AddApplicationServices();
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }

    internal class ConsoleCommandService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHostApplicationLifetime appLifetime;
        private readonly Type[] commandTypes;
        private readonly StringWriter helpWriter;

        public ConsoleCommandService(IServiceProvider serviceProvider, IHostApplicationLifetime appLifetime)
        {
            this.serviceProvider = serviceProvider;
            this.appLifetime = appLifetime;

            this.helpWriter = new StringWriter();
            this.commandTypes = this.GetType().Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsNested && p.GetCustomAttributes(typeof(VerbAttribute), false).Any())
                .ToArray();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await new Parser(settings =>
                {
                    settings.HelpWriter = this.helpWriter;
                    settings.IgnoreUnknownArguments = true;
                }).ParseArguments(Environment.GetCommandLineArgs().Skip(1), this.commandTypes)
                .WithNotParsed(this.Help)
                .WithParsedAsync<BaseCommand>(cmd => cmd.Execute(this.serviceProvider));

            this.appLifetime.StopApplication();
        }

        private void Help(IEnumerable<Error> errors)
        {
            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
            {
                var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
                Console.WriteLine($"Version: {version}");
            }
            else
                Console.WriteLine(this.helpWriter);
        }
    }
}
