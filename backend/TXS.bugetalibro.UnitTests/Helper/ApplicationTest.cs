using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Infrastructure;
using TXS.bugetalibro.Infrastructure.Persistence;
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

        private readonly CancellationTokenSource source = new CancellationTokenSource();

        public ApplicationFixture()
        {
            var env = new HostingEnvironment
            {
                EnvironmentName = Constants.Environment.Testing, 
                ContentRootPath = Environment.CurrentDirectory
            };

            var config = new ConfigurationBuilder()
                .Build();

            this.CreateSampleDb();
            
            this.Services = new ServiceCollection()
                .AddTransient<IHostEnvironment>( _ => env)
                .AddTransient<IConfiguration>( _ => config)
                .AddApplicationServices()
                .AddInfrastructureServices()
                .AddTestDateProvider()
                .AddInMemoryDb();
        }

        private void CreateSampleDb() {
            var services = new ServiceCollection()
                .AddInfrastructureServices()
                .AddSampleDb(TestOverrides.SampleDb);

            var sp = services
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider;
            
            sp.GetRequiredService<IDataStoreInitializer>().MigrateAsync().Wait(100);

            var dataStore = sp.GetRequiredService<IDataStore>();
            dataStore.Set<Einzahlung>().Insert(new Einzahlung(new DateTime(1968, 12, 24), 100.01m));
            dataStore.Set<Einzahlung>().Insert(new Einzahlung(new DateTime(2001, 10, 21), 200.99m));
            dataStore.Set<Einzahlung>().Insert(new Einzahlung(new DateTime(2006, 2, 20),  42.00m));

            dataStore.SaveChangesAsync(this.source.Token).Wait(100);
        }

        public void Dispose() {
            this.source.Dispose();
        }
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

        protected async Task UseSampleDb(string path) {
            var dataStoreContext = this.Get<DataStoreContext>();
            await dataStoreContext.Database.ExecuteSqlRawAsync($"attach '{TestOverrides.SampleDb}' as source");

            // Todo SB  Alle Tables in der richtigen Order von der persistierten Quelle nach :memory: kopieren
            await dataStoreContext.Database.ExecuteSqlRawAsync("insert into main.Einzahlung select * from source.Einzahlung");
        }
    }
}
