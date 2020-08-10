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
        void Insert(T entity);
        void InsertOrUpdate(T entity);
        void Update(Guid id, object values);
        void Delete(T entity);
    }    
}
