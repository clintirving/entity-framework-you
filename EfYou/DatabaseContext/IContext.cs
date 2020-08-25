// // -----------------------------------------------------------------------
// // <copyright file="IContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace EfYou.DatabaseContext
{
    public interface IContext : IDisposable
    {
        IDatabaseAccessor DatabaseAccessor { get; }
        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        DbEntityEntry Entry(object entity);
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        Type GetType();
        void SetState(object entity, EntityState state);
    }
}