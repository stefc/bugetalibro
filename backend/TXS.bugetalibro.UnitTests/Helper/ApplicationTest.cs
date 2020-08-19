using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Infrastructure;
using TXS.bugetalibro.Infrastructure.Persistence;

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
            // var dict = new Dictionary<string, string>
            // {
            //     {"ConnectionStrings:database", "Data Source=:memory:" }
            // };

            this.host = Host.CreateDefaultBuilder()
                .UseEnvironment(Constants.Environment.Testing)
                .ConfigureAppConfiguration( (context, config) => {
  //                  config.AddInMemoryCollection(dict);
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddApplicationServices()
                        .AddInfrastructureServices();
                    
                    // TODO SB Fluent Extension Method ?
                    services = services
                        .Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));
                        
                    this.MutateServiceCollection(services);
                })
                .Build();
            this.scope = this.host.Services.CreateScope();

            await this.Get<IDataStoreInitializer>().MigrateAsync();
            
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
