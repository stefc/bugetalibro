using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
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

        public const string SampleDb = "sample.db";

        public static IDataSet<T> CreateDataSet<T>(IList<T> container) where T : class
        {
            IQueryable<T> fakeIQueryable = new List<T>().AsQueryable();
            var fakeDbSet = A.Fake<IDataSet<T>>((d => d.Implements(typeof(IQueryable<T>))));

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).GetEnumerator())
                .Returns(fakeIQueryable.GetEnumerator());

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Provider)
                .Returns(fakeIQueryable.Provider);

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Expression)
                .Returns(fakeIQueryable.Expression);

            A.CallTo(() => ((IQueryable<T>)fakeDbSet).ElementType)
                .Returns(fakeIQueryable.ElementType);

            return fakeDbSet;
        }

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

    public static class OverridesExtensions
    {

        public static IServiceCollection AddTestDateProvider(this IServiceCollection services)
            => services.Replace(ServiceDescriptor.Transient<IDateProvider>(sp => new TestOverrides.DateProvider()));

        public static IServiceCollection AddInMemoryDb(this IServiceCollection services)
            => services
                .Replace(ServiceDescriptor.Scoped<DbContextOptions<DataStoreContext>>(_ => CreateInMemoryDb()))
                .Replace(ServiceDescriptor.Scoped<IDataStoreInitializer>(
                    sp => new TestOverrides.DataStoreInitializer(sp.GetRequiredService<DataStoreContext>())));

        public static IServiceCollection AddSampleDb(this IServiceCollection services, string path)
            => services
                .Replace(ServiceDescriptor.Scoped<DbContextOptions<DataStoreContext>>(_ => CreateNewSampleDb(path)))
                .Replace(ServiceDescriptor.Scoped<IDataStoreInitializer>(
                    sp => new TestOverrides.DataStoreInitializer(sp.GetRequiredService<DataStoreContext>())));



        private static DbContextOptions<DataStoreContext> CreateInMemoryDb()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<DataStoreContext>().UseSqlite(connection).Options;

            return options;
        }

        private static DbContextOptions<DataStoreContext> CreateNewSampleDb(string path)
        {
            File.Delete(path);
            var connection = new SqliteConnection($"DataSource={path}");
            connection.Open();

            return new DbContextOptionsBuilder<DataStoreContext>().UseSqlite(connection).Options;
        }


    }
}
