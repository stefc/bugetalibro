using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TXS.bugetalibro.Application.Contracts.Data;

namespace TXS.bugetalibro.Infrastructure.Persistence
{
    internal class DataStoreInitializer : IDataStoreInitializer
    {
        private readonly DataStoreContext dataStoreContext;

        public DataStoreInitializer(DataStoreContext dataStoreContext)
        {
            this.dataStoreContext = dataStoreContext;
        }

        async Task IDataStoreInitializer.MigrateAsync() 
            => await this.dataStoreContext.Database.MigrateAsync();
    }
}
