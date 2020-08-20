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
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IHostApplicationLifetime appLifetime;

        public CliService(IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime appLifetime)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
            using (var scope = this.serviceScopeFactory.CreateScope())
            {
                var commandLauncher = scope.ServiceProvider.GetRequiredService<CommandLauncher>();
                await commandLauncher.ExecuteCommandAsync(commandLineArgs, stoppingToken);
            }
            this.appLifetime.StopApplication();
        }
    }
}
