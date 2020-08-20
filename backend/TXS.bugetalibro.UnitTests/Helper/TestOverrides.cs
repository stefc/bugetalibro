using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Infrastructure.Persistence;

namespace TXS.bugetalibro.UnitTests.Helper
{
    internal static class TestOverrides
    {
        public class DateProvider : IDateProvider
        {
            public DateTime Today { get; }

            public DateProvider(DateTime? date = null)
            {
                this.Today = date ?? new DateTime(2019, 12, 24);
            }
        }

         public class DataStoreInitializer : IDataStoreInitializer
    {
            private readonly DataStoreContext dataStoreContext;

            public DataStoreInitializer(DataStoreContext dataStoreContext)
            {
                this.dataStoreContext = dataStoreContext;
            }

            async Task IDataStoreInitializer.MigrateAsync() 
                => await this.dataStoreContext.Database.EnsureCreatedAsync();
        }
    }

    public static class OverridesExtensions {

        public static IServiceCollection AddTestDateProvider(this IServiceCollection services)
            => services.Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));

        public static IServiceCollection AddInMemoryDb(this IServiceCollection services)
            => services
                .Replace(ServiceDescriptor.Scoped<DbContextOptions<DataStoreContext>>( _ => CreateInMemoryDb()))
                .Replace(ServiceDescriptor.Scoped<IDataStoreInitializer>( 
                    sp => new TestOverrides.DataStoreInitializer(sp.GetRequiredService<DataStoreContext>())));


        private static DbContextOptions<DataStoreContext> CreateInMemoryDb()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DataStoreContext>().UseSqlite(connection).Options;

            return options;
        }

    }
}
