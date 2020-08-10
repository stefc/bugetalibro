using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using TXS.bugetalibro.Application;
using TXS.bugetalibro.Application.Contracts;
using TXS.Shared.Helper;

using Xunit;

namespace TXS.bugetalibro.UnitTests.Helper
{
    public abstract class ApplicationTest : IAsyncLifetime
    {
        private static readonly AsyncLock mutex = new AsyncLock();
        private static readonly IServiceCollection baseServices;
        private IServiceProvider serviceProvider;

        protected TService Get<TService>() => this.serviceProvider.GetService<TService>();
        protected IMediator Mediator => this.Get<IMediator>();

        static ApplicationTest()
        {
            var testConfiguration = new ConfigurationBuilder().Build();

            baseServices = new ServiceCollection();
            baseServices.AddTransient<IHostEnvironment>(sp => new HostingEnvironment
            {
                EnvironmentName = Constants.Environment.Testing,
                ContentRootPath = Environment.CurrentDirectory
            });

            baseServices.AddTransient<IConfiguration>(sp => testConfiguration);
            
            baseServices.AddApplication();
            
            baseServices.Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));
        }

        protected ApplicationTest() { }

        async Task IAsyncLifetime.InitializeAsync()
        {
            using (await mutex.LockAsync())
            {
                var serviceCollection = new ServiceCollection().Add(baseServices);
                this.MutateServiceCollection(serviceCollection);
                this.serviceProvider = serviceCollection.BuildServiceProvider();
            }
        }

        Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

        protected virtual void MutateServiceCollection(IServiceCollection services) { }
    }
}
