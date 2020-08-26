using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.Shared.Extensions;

namespace TXS.bugetalibro.Infrastructure.Persistence
{
    internal class DataStoreContext : DbContext, IDataStore
    {
        public DataStoreContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(DataStoreContext).Assembly)
                // Sqlite unterstützt als Fließkommatyp nur double, deswegen werden alle
                // decimal Properties Richtung Sqlite zu double konvertiert.
                .Model.GetEntityTypes()
                    .SelectMany(entityType => entityType.ClrType.GetProperties()
                        .Where(prop => prop.PropertyType == typeof(decimal))
                        .Select(prop => new {ModelName = entityType.Name, PropertyName = prop.Name}))
                    .ForEach(entityProperty =>
                        modelBuilder.Entity(entityProperty.ModelName).Property(entityProperty.PropertyName)
                            .HasConversion<double>());
        }  

        IDataSet<T> IDataStore.Set<T>() => new SetDecorator<T>(this);
        
        private class SetDecorator<T> : IDataSet<T>
            where T : class
        {
            private readonly DbContext dbContext;
            private readonly DbSet<T> dbSet;

            public SetDecorator(DbContext dbContext)
            {
                this.dbContext = dbContext;
                this.dbSet = dbContext.Set<T>();
            }

            void IDataSet<T>.Insert(T entity) => this.dbContext.Add(entity);

            void IDataSet<T>.InsertOrUpdate(T entity) => this.dbContext.Update(entity);

            void IDataSet<T>.Delete(T entity) => this.dbContext.Remove(entity);

            void IDataSet<T>.Update(Guid id, object values)
            {
                var existingEntity = this.dbContext.Find<T>(id);
                if (existingEntity == null)
                    throw new KeyNotFoundException($"Key '{id}' in table '{typeof(T).Name}' not found.");

                this.dbContext.Entry(existingEntity).CurrentValues.SetValues(values);
            }

            #region IQueryable, IAsyncEnumerable implementation

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)this.dbSet).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.dbSet).GetEnumerator();

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
                => ((IAsyncEnumerable<T>)this.dbSet).GetAsyncEnumerator(cancellationToken);

            Type IQueryable.ElementType => ((IQueryable<T>)this.dbSet).ElementType;
            Expression IQueryable.Expression => ((IQueryable<T>)this.dbSet).Expression;
            IQueryProvider IQueryable.Provider => ((IQueryable<T>)this.dbSet).Provider;

            #endregion
        }
    }
}
