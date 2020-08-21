using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Infrastructure;

using Xunit;

namespace TXS.bugetalibro.UnitTests.Helper
{

    [CollectionDefinition(nameof(ApplicationFixtureCollection))]
    public class ApplicationFixtureCollection : ICollectionFixture<ApplicationFixture>
    {
    }

    public class ApplicationFixture : IDisposable
    {
        public readonly IServiceCollection Services;

        public ApplicationFixture()
        {
            var env = new HostingEnvironment
            {
                EnvironmentName = Constants.Environment.Testing, 
                ContentRootPath = Environment.CurrentDirectory
            };

            var config = new ConfigurationBuilder()
                .Build();
            
            this.Services = new ServiceCollection()
                .AddTransient<IHostEnvironment>( _ => env)
                .AddTransient<IConfiguration>( _ => config)
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddTestDateProvider()
                .AddInMemoryDb();
        }

        public void Dispose() {}
    }

    [Collection(nameof(ApplicationFixtureCollection))]
    [Trait("Category","Integration")]

    // Basis für Application Tests
    public abstract class ApplicationTest : IAsyncLifetime
    {
        private readonly ApplicationFixture fixture; 

        private IServiceProvider serviceProvider;

        protected TService Get<TService>() => this.serviceProvider.GetRequiredService<TService>();
        protected IMediator Mediator => this.Get<IMediator>();

        public ApplicationTest(ApplicationFixture fixture)
        {
            this.fixture = fixture;
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            this.serviceProvider = this.fixture.Services
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider;

            await this.Get<IDataStoreInitializer>().MigrateAsync();
        }

         Task IAsyncLifetime.DisposeAsync() {
            return Task.CompletedTask;
        }

        protected virtual void MutateServiceCollection(IServiceCollection services) { }
    }
}
