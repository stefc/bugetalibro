using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TXS.bugetalibro.ConsoleApp.Infrastructure
{
    internal class CliService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceProvider;
        private readonly CommandLauncher commandLauncher;
        private readonly IHostApplicationLifetime appLifetime;

        public CliService(IServiceScopeFactory serviceProvider, CommandLauncher commandLauncher, IHostApplicationLifetime appLifetime)
        {
            this.serviceProvider = serviceProvider;
            this.commandLauncher = commandLauncher;
            this.appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
            await this.commandLauncher.ExecuteCommandAsync(commandLineArgs, stoppingToken);
            this.appLifetime.StopApplication();
        }
    }
}
