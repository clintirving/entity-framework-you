// // -----------------------------------------------------------------------
// // <copyright file="IContext.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfYou.DatabaseContext
{
    public interface IContext : IDisposable
    {
        IDatabaseAccessor DatabaseAccessor { get; }
        ChangeTracker ChangeTracker { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
        EntityEntry Entry(object entity);
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        Type GetType();
        void SetState(object entity, EntityState state);
        IModel Model { get; }

    }
}