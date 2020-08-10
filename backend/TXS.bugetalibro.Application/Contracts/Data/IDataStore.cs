using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TXS.bugetalibro.Application.Contracts.Data
{
    public interface IDataStore
    {
        IDataSet<T> Set<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
    
    public interface IDataSet<T> : IQueryable<T>, IAsyncEnumerable<T>
        where T : class
    {
        Lazy<int> Insert(T entity);
        Lazy<int> InsertOrUpdate(T entity);
        void Update(int id, object values);
        void Delete(T entity);
    }    
}
