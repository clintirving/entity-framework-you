// // -----------------------------------------------------------------------
// // <copyright file="Context.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Common.Logging;
using EfYouCore.Extensions;
using EfYouCore.Model.Attributes;
using EfYouCore.Model.Enumerations;
using EfYouCore.Model.Models;
using EfYouCore.Security.User;
using EfYouCore.Utilities;

namespace EfYouCore.DatabaseContext
{
    public class Context : DbContext, IContext
    {
        private readonly IIdentityService _identityService;
        private readonly ILog _log;

        public Context(string databaseName, IIdentityService identityService, ILog log)
            : this(databaseName)
        {
            _identityService = identityService;
            _log = log;
            DatabaseAccessor = new DatabaseAccessor(Database);
        }

        public Context(string databaseName)
            : base(databaseName)
        {
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Audit> Audits { get; set; }

        public IDatabaseAccessor DatabaseAccessor { get; }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            var changedEntities = ChangeTracker.Entries().ToList();
            var originalStates = changedEntities.Select(x => x.State).ToList();

            var result = base.SaveChanges();

            SaveAudit(changedEntities, originalStates);

            return result;
        }

        public void SetState(object entity, EntityState state)
        {
            Entry(entity).State = state;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<ForeignKeyNamingConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void SaveAudit(IEnumerable<DbEntityEntry> dbEntityEntries, IEnumerable<EntityState> originalEntityStates)
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
                TypeId = (int) entity.GetIdFromEntity(),
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