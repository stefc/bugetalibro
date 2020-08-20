using System;
using System.IO;
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
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json").Build();
            
            this.Services = new ServiceCollection()
                .AddTransient<IHostEnvironment>( _ => env)
                .AddTransient<IConfiguration>( _ => config)
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddTestDateProvider();
        }

        public void Dispose() {}
    }

    [Collection(nameof(ApplicationFixtureCollection))]

    // Basis für Application Tests
    public abstract class ApplicationTest : IAsyncLifetime, IDisposable
    {
        private readonly ApplicationFixture fixture; 

        private readonly string dbPath;

        private IServiceProvider serviceProvider;

        protected TService Get<TService>() => this.serviceProvider.GetRequiredService<TService>();
        protected IMediator Mediator => this.Get<IMediator>();

        public ApplicationTest(ApplicationFixture fixture)
        {
            this.fixture = fixture;

            var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            this.dbPath = Path.ChangeExtension(tempFile, ".db");
        }

        public void Dispose()
        {
            File.Delete(this.dbPath);
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            this.serviceProvider = this.fixture.Services
                
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider;

            var config = this.Get<IConfiguration>();
            config["ConnectionStrings:database"] = $"Data Source={dbPath}";

            await this.Get<IDataStoreInitializer>().MigrateAsync();
        }

         Task IAsyncLifetime.DisposeAsync() {
            return Task.CompletedTask;
        }

        protected virtual void MutateServiceCollection(IServiceCollection services) { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices();

        }

    }
}
