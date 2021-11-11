using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EfYou.CascadeDelete;
using EfYou.DatabaseContext;
using EfYou.Extensions;
using EfYou.Filters;
using EfYou.Permissions;
using EfYou.ScopeOfResponsibility;

namespace EfYou.EntityServices
{
    public class TimescaleEntityService<T> : EntityService<T>, ITimescaleEntityService<T> where T : class, new()
    {
        private readonly ITimescaleContextFactory _timescaleContextFactory;
        private readonly IPermissionService<T> _permissionService;
        private readonly IScopeOfResponsibilityService<T> _scopeOfResponsibilityService;
        private readonly IFilterService<T> _filterService;
        private readonly ICascadeDeleteService<T> _cascadeDeletionService;

        private readonly Paging _selectTop1 = new Paging { Count = 1, Page = 0 };

        public TimescaleEntityService(ITimescaleContextFactory timescaleContextFactory, IFilterService<T> filterService, ICascadeDeleteService<T> cascadeDeletionService, IPermissionService<T> permissionService, IScopeOfResponsibilityService<T> scopeOfResponsibilityService) 
            : base(timescaleContextFactory, filterService, cascadeDeletionService, permissionService, scopeOfResponsibilityService)
        {
            _timescaleContextFactory = timescaleContextFactory;
            _permissionService = permissionService;
            _scopeOfResponsibilityService = scopeOfResponsibilityService;
            _filterService = filterService;
        }

        public virtual List<T> Get(List<DateTime> intervals)
        {
            return Get(intervals, null, null, null);
        }

        public virtual List<T> Get(List<DateTime> intervals, List<string> includes)
        {
            return Get(intervals, includes, null, null);
        }

        public virtual List<T> Get(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(intervals, includes, orderBys, null);
        }

        public virtual T GetFirst(List<DateTime> intervals)
        {
            return GetFirst(intervals, null, null);
        }

        public virtual T GetFirst(List<DateTime> intervals, List<string> includes)
        {
            return GetFirst(intervals, includes, null);
        }

        public virtual T GetFirst(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(intervals, includes, orderBys, _selectTop1).FirstOrDefault();
        }

        public virtual List<T> Get(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            _permissionService.Get();

            using (var context = _timescaleContextFactory.Create())
            {
                IQueryable<T> query = context.Set<T>();

                query = _scopeOfResponsibilityService.FilterResultOnCurrentPrincipal(query);

                query = _filterService.FilterResultsOnGet(query, intervals);

                query = _filterService.AddIncludes(query, includes);

                query = _filterService.AddOrderBys(query, orderBys);

                query = _filterService.AddPaging(query, paging);

                var results = query.ToList();

                return results;
            }
        }

        public virtual void Delete(List<DateTime> ids)
        {
            _permissionService.Delete();

            // filter via Get method for scope of responsibility.
            var entitiesToDelete = Get(ids);

            if (entitiesToDelete.Count != 0)
            {
                _cascadeDeletionService.CascadeDelete(entitiesToDelete.GetIdsFromEntities());

                using (var context = _timescaleContextFactory.Create())
                {
                    var dbSet = context.Set<T>();

                    foreach (var entityToDelete in entitiesToDelete)
                    {
                        dbSet.Attach(entityToDelete);
                        context.SetState(entityToDelete, EntityState.Deleted);
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException exception)
                    {
                        WrapValidationException(exception);
                    }
                }
            }
        }

        // Intervals being passed in should match the form of:
        //      1 day
        //      2 months
        //      28 days
        //      14 years
        public void DeleteChunks(string interval)
        {
            var tableAttribute = (TableAttribute)typeof(T).GetProperties()
                .FirstOrDefault(x => x.GetCustomAttribute(typeof(TableAttribute)) != null).GetCustomAttribute(typeof(TableAttribute));

            if (tableAttribute == null)
            {
                throw new ApplicationException("Table attribute required for DeleteChunks");
            }

            using var context = _timescaleContextFactory.Create();

            context.DatabaseAccessor.ExecuteSqlCommand("SELECT drop_chunks('@p0','@p1'", tableAttribute.Name, interval);
        }
    }
}