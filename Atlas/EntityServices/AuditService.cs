// // -----------------------------------------------------------------------
// // <copyright file="AuditService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Atlas.CascadeDelete;
using Atlas.DatabaseContext;
using Atlas.Filters;
using Atlas.Model.Models;
using Atlas.Permissions;
using Atlas.ScopeOfResponsibility;

namespace Atlas.EntityServices
{
    public class AuditService : EntityService<Audit>
    {
        public AuditService(IContextFactory contextFactory, IFilterService<Audit> filterService, ICascadeDeleteService<Audit> cascadeDeletionService,
            IPermissionService<Audit> permissionService, IScopeOfResponsibilityService<Audit> scopeOfResponsibilityService)
            : base(contextFactory, filterService, cascadeDeletionService, permissionService, scopeOfResponsibilityService)
        {
        }

        public override List<Audit> Add(List<Audit> entitiesToAdd)
        {
            throw new NotSupportedException("Cannot add directly to the audits");
        }

        public override void Delete(List<long> ids)
        {
            throw new NotSupportedException("Cannot delete audits");
        }

        public override void Update(List<Audit> items)
        {
            throw new NotSupportedException("Cannot update audits");
        }
    }
}