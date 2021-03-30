// // -----------------------------------------------------------------------
// // <copyright file="EntityServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using EfYou.CascadeDelete;
using EfYou.DatabaseContext;
using EfYou.Extensions;
using EfYou.Filters;
using EfYou.Permissions;
using EfYou.ScopeOfResponsibility;
using EfYou.Utilities;

namespace EfYou.EntityServices
{
    public class EntityService<T> : IEntityService<T> where T : class, new()
    {
        private readonly ICascadeDeleteService<T> _cascadeDeletionService;
        private readonly IContextFactory _contextFactory;
        private readonly IFilterService<T> _filterService;
        private readonly IPermissionService<T> _permissionService;
        private readonly IScopeOfResponsibilityService<T> _scopeOfResponsibilityService;

        private readonly Paging _selectTop1 = new Paging {Count = 1, Page = 0};

        public EntityService(IContextFactory contextFactory, IFilterService<T> filterService,
            ICascadeDeleteService<T> cascadeDeletionService, IPermissionService<T> permissionService,
            IScopeOfResponsibilityService<T> scopeOfResponsibilityService)
        {
            _contextFactory = contextFactory;
            _filterService = filterService;
            _cascadeDeletionService = cascadeDeletionService;
            _permissionService = permissionService;
            _scopeOfResponsibilityService = scopeOfResponsibilityService;
        }

        public virtual List<T> Get(List<long> ids)
        {
            return Get(ids, null, null, null);
        }

        public virtual List<T> Get(List<long> ids, List<string> includes)
        {
            return Get(ids, includes, null, null);
        }

        public virtual List<T> Get(List<long> ids, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(ids, includes, orderBys, null);
        }

        public virtual T GetFirst(List<long> ids)
        {
            return GetFirst(ids, null, null);
        }

        public virtual T GetFirst(List<long> ids, List<string> includes)
        {
            return GetFirst(ids, includes, null);
        }

        public virtual T GetFirst(List<long> ids, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(ids, includes, orderBys, _selectTop1).FirstOrDefault();
        }

        public virtual List<T> Get(List<long> ids, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            _permissionService.Get();

            using (var context = _contextFactory.Create())
            {
                IQueryable<T> query = context.Set<T>();

                query = _scopeOfResponsibilityService.FilterResultOnCurrentPrincipal(query);

                query = _filterService.FilterResultsOnGet(query, ids);

                query = _filterService.AddIncludes(query, includes);

                query = _filterService.AddOrderBys(query, orderBys);

                query = _filterService.AddPaging(query, paging);

                var results = query.ToList();

                return results;
            }
        }

        public virtual List<T> Search(List<T> filters)
        {
            return Search(filters, null, null, null);
        }

        public virtual List<T> Search(List<T> filters, List<string> includes)
        {
            return Search(filters, includes, null, null);
        }

        public virtual List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys)
        {
            return Search(filters, includes, orderBys, null);
        }

        public virtual List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            _permissionService.Search();

            using (var context = _contextFactory.Create())
            {
                var searchQuery = CreateSearchQuery(context, filters, includes, orderBys, paging);

                if (searchQuery != null)
                {
                    return searchQuery.ToList();
                }
            }

            return new List<T>();
        }

        public virtual List<List<long>> SearchAggregate(List<T> filters, List<string> aggregate)
        {
            return SearchAggregate(filters, null, null, null, aggregate);
        }

        public virtual List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<string> aggregate)
        {
            return SearchAggregate(filters, includes, null, null, aggregate);
        }

        public virtual List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, List<string> aggregate)
        {
            return SearchAggregate(filters, includes, orderBys, null, aggregate);
        }

        public virtual List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging,
            List<string> groupBys)
        {
            _permissionService.Search();

            using (var context = _contextFactory.Create())
            {
                var aggregateQuery = CreateSearchAggregateQuery(context, filters, includes, orderBys, paging, groupBys);

                if (aggregateQuery != null)
                {
                    var resultSet = aggregateQuery.ToList();

                    return resultSet.SelectMany(x => x).ToList();
                }
            }

            return new List<List<long>>();
        }


        public virtual T SearchFirst(List<T> filters)
        {
            return SearchFirst(filters, null, null);
        }

        public virtual T SearchFirst(List<T> filters, List<string> includes)
        {
            return SearchFirst(filters, includes, null);
        }

        public virtual T SearchFirst(List<T> filters, List<string> includes, List<OrderBy> orderBys)
        {
            return Search(filters, includes, orderBys, _selectTop1).FirstOrDefault();
        }

        public virtual long SearchCount(List<T> filters)
        {
            _permissionService.Search();

            return SearchResultCountOnly(filters);
        }

        public virtual long SearchAggregateCount(List<T> filters, List<string> groupBys)
        {
            _permissionService.Search();

            return SearchAggregateResultCountOnly(filters, groupBys);
        }

        public virtual List<T> Add(List<T> entitiesToAdd)
        {
            _permissionService.Add();

            EntityExtensions.SetDefaultValuesOnEntities(entitiesToAdd);

            using (var context = _contextFactory.Create())
            {
                var dbSet = context.Set<T>();
                dbSet.AddRange(entitiesToAdd);

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException exception)
                {
                    WrapValidationException(exception);
                }
            }

            return entitiesToAdd;
        }

        public virtual void Delete(List<long> ids)
        {
            _permissionService.Delete();

            // filter via Get method for scope of responsibility.
            var entitiesToDelete = Get(ids);

            if (entitiesToDelete.Count != 0)
            {
                _cascadeDeletionService.CascadeDelete(entitiesToDelete.GetIdsFromEntities());

                using (var context = _contextFactory.Create())
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

        public virtual void Update(List<T> items)
        {
            _permissionService.Update();

            // filter via Get method for scope of responsibility.

            var itemsWithIds = items.Select(x => new {Id = x.GetIdFromEntity(), Entity = x}).ToList();

            var entityIdsToUpdate = Get(itemsWithIds.Select(x => x.Id).ToList()).GetIdsFromEntities();

            var entitiesToUpdate = itemsWithIds.Where(x => entityIdsToUpdate.Contains(x.Id)).Select(x => x.Entity);

            using (var context = _contextFactory.Create())
            {
                var dbSet = context.Set<T>();

                foreach (var entity in entitiesToUpdate)
                {
                    dbSet.Attach(entity);
                    context.SetState(entity, EntityState.Modified);
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

        protected IQueryable<IGrouping<long, List<long>>> CreateSearchAggregateQuery(IContext context, List<T> filters, List<string> includes,
            List<OrderBy> orderBys, Paging paging, List<string> groupBys)
        {
            var query = CreateSearchQuery(context, filters, includes, orderBys, null);

            var aggregateQuery = _filterService.AddAggregationFilter(query, groupBys, paging, orderBys);

            return aggregateQuery;
        }

        protected virtual long SearchResultCountOnly(IEnumerable<T> filters)
        {
            using (var context = _contextFactory.Create())
            {
                var searchQuery = CreateSearchQuery(context, filters, null, null, null);

                if (searchQuery != null)
                {
                    return searchQuery.Count();
                }
            }

            return 0;
        }

        protected virtual long SearchAggregateResultCountOnly(List<T> filters, List<string> groupBys)
        {
            using (var context = _contextFactory.Create())
            {
                var searchAggregateQuery = CreateSearchAggregateQuery(context, filters, null, new List<OrderBy>(), null, groupBys);

                if (searchAggregateQuery != null)
                {
                    return searchAggregateQuery.Count();
                }
            }

            return 0;
        }

        protected virtual IQueryable<T> CreateSearchQuery(IContext context, IEnumerable<T> filters, List<string> includes, List<OrderBy> orderBys,
            Paging paging)
        {
            IQueryable<T> completeQuery = null;

            foreach (var filter in filters)
            {
                IQueryable<T> queryForFilter = context.Set<T>();

                queryForFilter = _scopeOfResponsibilityService.FilterResultOnCurrentPrincipal(queryForFilter);

                queryForFilter = _filterService.FilterResultsOnSearch(queryForFilter, filter);

                queryForFilter = _filterService.AddIncludes(queryForFilter, includes);

                completeQuery = completeQuery == null ? queryForFilter : completeQuery.Concat(queryForFilter);
            }

            if (completeQuery != null)
            {
                completeQuery = completeQuery.Distinct();

                completeQuery = _filterService.AddOrderBys(completeQuery, orderBys);

                completeQuery = _filterService.AddPaging(completeQuery, paging);
            }

            return completeQuery;
        }

        protected virtual void WrapValidationException(DbEntityValidationException exception)
        {
            throw new ApplicationException("Validation errors have occurred. " +
                                           string.Join("; ",
                                               exception.EntityValidationErrors.SelectMany(x => x.ValidationErrors)
                                                   .Select(
                                                       x =>
                                                           string.Format("Property: {0}, Error: {1}", x.PropertyName,
                                                               x.ErrorMessage))));
        }
    }
}