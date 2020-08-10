using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TXS.bugetalibro.Application;

namespace TXS.bugetalibro.Infrastructure.Persistence
{
    internal static class DataStoreConnectionSettings
    {
        public static void Setup(IServiceProvider serviceProvider, DbContextOptionsBuilder builder)
        {
            builder.Configure(serviceProvider.GetService<IConfiguration>())
                .UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>())
                .EnableSensitiveDataLogging();
        }

        public static DbContextOptionsBuilder Configure(this DbContextOptionsBuilder builder,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(Constants.ConnectionStringKeys.Database);
            builder.UseSqlite(connectionString);

            return builder;
        }
    }
}
