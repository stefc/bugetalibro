using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Infrastructure.Persistence;
using TXS.bugetalibro.Infrastructure.Services;

namespace TXS.bugetalibro.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services
                .AddDbContextPool<DataStoreContext>(DataStoreConnectionSettings.Setup)
                .AddTransient<IDataStoreInitializer, DataStoreInitializer>()
                .AddTransient<IDateProvider, SystemDateProvider>();
        }
    }
}
