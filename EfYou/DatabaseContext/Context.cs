// // -----------------------------------------------------------------------
// // <copyright file="Context.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfYou.Extensions;
using EfYou.Model.Attributes;
using EfYou.Model.Enumerations;
using EfYou.Model.Models;
using EfYou.Security.User;
using EfYou.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfYou.DatabaseContext
{
    public class Context : DbContext, IContext
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger _log;
        private IDatabaseAccessor _databaseAccessor;

        public Context(DbContextOptions dbContextOptions, IIdentityService identityService, ILogger log)
            : this(dbContextOptions)
        {
            _identityService = identityService;
            _log = log;
        }

        public Context(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public IDatabaseAccessor DatabaseAccessor
        {
            get { return _databaseAccessor ??= new DatabaseAccessor(Database); }
        }

        public virtual DbSet<Audit> Audits { get; set; }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
        {
            DetectChangesAndSaveAudit();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DetectChangesAndSaveAudit();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void DetectChangesAndSaveAudit()
        {
            ChangeTracker.DetectChanges();

            var changedEntities = ChangeTracker.Entries().ToList();
            var originalStates = changedEntities.Select(x => x.State).ToList();

            var result = base.SaveChanges();

            SaveAudit(changedEntities, originalStates);
        }


        public override async Task<int> SaveChangesAsync()
        {
            ChangeTracker.DetectChanges();

            var changedEntities = ChangeTracker.Entries().ToList();
            var originalStates = changedEntities.Select(x => x.State).ToList();

            var result = await base.SaveChangesAsync();

            SaveAudit(changedEntities, originalStates);

            return result;
        }

        public void SetState(object entity, EntityState state)
        {
            Entry(entity).State = state;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys())
                         .Where(x => !x.IsOwnership && x.DeleteBehavior == DeleteBehavior.Cascade))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        protected virtual void SaveAudit(IEnumerable<EntityEntry> dbEntityEntries, IEnumerable<EntityState> originalEntityStates)
        {
            var entitiesWithOriginalStates = dbEntityEntries.Zip(originalEntityStates, (x, y) => new {x.Entity, State = y});

            var audits = entitiesWithOriginalStates
                .Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached &&
                            x.Entity.GetType().IsDefined(typeof(AuditMeAttribute), false)).Select(x => CreateAudit(x.Entity, x.State)).ToList();

            Audits.AddRange(audits);
            base.SaveChanges();
        }

        protected Audit CreateAudit(object entity, EntityState entityState)
        {
            if (entity.GetType().GetPrimaryKeyProperty().PropertyType == typeof(long))
            {
                throw new ApplicationException("Cannot Audit a class with a primary key of type long");
            }

            return new Audit
            {
                AuditAction = EntityStateToAuditAction(entityState),
                DateTime = DateTime.UtcNow,
                Email = _identityService.GetEmail(),
                Type = entity.GetType().FullName,
                TypeId = entity.GetIdFromEntity().ToString(),
                SerializedEntity = entity.SerializeToXml()
            };
        }

        protected AuditAction EntityStateToAuditAction(EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Added:
                    return AuditAction.Add;
                case EntityState.Deleted:
                    return AuditAction.Delete;
                case EntityState.Modified:
                    return AuditAction.Update;
                default:
                    return AuditAction.Fetch;
            }
        }
    }
}