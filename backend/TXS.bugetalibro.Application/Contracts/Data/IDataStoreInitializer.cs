using System.Threading.Tasks;

namespace TXS.bugetalibro.Application.Contracts.Data
{
    public interface IDataStoreInitializer
    {
        Task MigrateAsync();
    }
}
