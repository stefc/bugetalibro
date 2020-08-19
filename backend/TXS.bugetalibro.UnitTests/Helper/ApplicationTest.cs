using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Infrastructure;

using Xunit;

namespace TXS.bugetalibro.UnitTests.Helper
{
    // Basis für Application Tests
    public abstract class ApplicationTest : IAsyncLifetime
    {
        private IHost host;
        private IServiceScope scope;

        protected TService Get<TService>() => this.scope.ServiceProvider.GetRequiredService<TService>();
        protected IMediator Mediator => this.Get<IMediator>();

        protected ApplicationTest() { }

        async Task IAsyncLifetime.InitializeAsync()
        {
            this.host = Host.CreateDefaultBuilder()
                .UseEnvironment(Constants.Environment.Testing)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddApplicationServices()
                        .AddInfrastructureServices()
                        .Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));
                    this.MutateServiceCollection(services);
                })
                .Build();
            this.scope = this.host.Services.CreateScope();
            await this.host.StartAsync();
        }

         Task IAsyncLifetime.DisposeAsync() {
            this.scope?.Dispose();
            this.scope=null;
            this.host?.Dispose();
            this.host=null;
            return Task.CompletedTask;
        }

        protected virtual void MutateServiceCollection(IServiceCollection services) { }
    }
}
