using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TXS.bugetalibro.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Wird nur für das erzeugen des <see cref="DataStoreContext"/> zur DesignTime genutzt.
    /// Im speziellen für das erstellen der Migrationen per dotnet tool ef-migrations
    /// </summary>
    internal class MigrationsContextFactory : IDesignTimeDbContextFactory<DataStoreContext>
    {
        DataStoreContext IDesignTimeDbContextFactory<DataStoreContext>.CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DataStoreContext>().Configure(configuration);
            return new DataStoreContext(optionsBuilder.Options);
        }
    }
}
