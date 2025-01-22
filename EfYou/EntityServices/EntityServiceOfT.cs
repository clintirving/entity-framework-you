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
using System.Transactions;
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

        /// <summary>
        /// If True DELETE skips Audit regardless of whether AuditMe is specified on the Entity and deletes directly in the DB with a SQL Command.
        /// </summary>
        public virtual bool UseBulkDelete { get; }

        public virtual List<T> Get(List<dynamic> ids)
        {
            return Get(ids, null, null, null);
        }

        public virtual List<T> Get(List<dynamic> ids, List<string> includes)
        {
            return Get(ids, includes, null, null);
        }

        public virtual List<T> Get(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(ids, includes, orderBys, null);
        }

        public virtual T GetFirst(List<dynamic> ids)
        {
            return GetFirst(ids, null, null);
        }

        public virtual T GetFirst(List<dynamic> ids, List<string> includes)
        {
            return GetFirst(ids, includes, null);
        }

        public virtual T GetFirst(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys)
        {
            return Get(ids, includes, orderBys, _selectTop1).FirstOrDefault();
        }

        public virtual List<T> Get(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            using var context = _contextFactory.Create();

            var query = QueryableGet(context, ids, includes, orderBys, paging);

            query = query.AsNoTracking();

            return query.ToList();
        }

        public virtual IQueryable<T> QueryableGet(IContext context, List<dynamic> ids)
        {
            return QueryableGet(context, ids, null);
        }

        public virtual IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes)
        {
            return QueryableGet(context, ids, includes, null);
        }

        public virtual IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes, List<OrderBy> orderBys)
        {
            return QueryableGet(context, ids, includes, orderBys, null);
        }

        public virtual IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            _permissionService.Get();

            IQueryable<T> query = context.Set<T>();

            query = _scopeOfResponsibilityService.FilterResultOnCurrentPrincipal(query);

            query = _filterService.FilterResultsOnGet(query, ids, context);

            query = _filterService.AddIncludes(query, includes, context);

            query = _filterService.AddOrderBys(query, orderBys, context);

            query = _filterService.AddPaging(query, paging, context);

            return query;
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
            using var context = _contextFactory.Create();

            var query = QueryableSearch(context, filters, includes, orderBys, paging);

            if (query != null)
            {
                query = query.AsNoTracking();

                return query.ToList();
            }

            return new List<T>();
        }

        public virtual IQueryable<T> QueryableSearch(IContext context, List<T> filters)
        {
            return QueryableSearch(context, filters, null);
        }

        public virtual IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes)
        {
            return QueryableSearch(context, filters, includes, null);
        }

        public virtual IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys)
        {
            return QueryableSearch(context, filters, includes, orderBys, null);
        }

        public virtual IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            _permissionService.Search();

            return CreateSearchQuery(context, filters, includes, orderBys, paging);
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

        public virtual List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys)
        {
            using (var context = _contextFactory.Create())
            {
                var query = QueryableSearchAggregate(context, filters, includes, orderBys, paging, groupBys);

                if (query != null)
                {
                    var resultSet = query.ToList();

                    return resultSet.SelectMany(x => x).ToList();
                }
            }

            return new List<List<long>>();
        }

        public virtual IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters)
        {
            return QueryableSearchAggregate(context, filters, null);
        }

        public virtual IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes)
        {
            return QueryableSearchAggregate(context, filters, includes, null);
        }

        public virtual IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys)
        {
            return QueryableSearchAggregate(context, filters, includes, orderBys, null);
        }

        public virtual IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging)
        {
            return QueryableSearchAggregate(context, filters, includes, orderBys, paging, null);
        }

        public virtual IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys)
        {
            _permissionService.Search();

            return CreateSearchAggregateQuery(context, filters, includes, orderBys, paging, groupBys);
        }

        public virtual T SearchFirst(List<T> filters)
        {
            return SearchFirst(filters, null);
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

            using var context = _contextFactory.Create();

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

            return entitiesToAdd;
        }

        public virtual void Delete(List<dynamic> ids)
        {
            _permissionService.Delete();

            // filter via Get method for scope of responsibility.
            var entitiesToDelete = Get(ids);

            if (entitiesToDelete.Count != 0)
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    _cascadeDeletionService.CascadeDelete(entitiesToDelete.GetIdsFromEntities());

                    if (UseBulkDelete)
                    {
                        DeleteUsingBulkDelete(entitiesToDelete);
                    }
                    else
                    {
                        DeleteUsingEntityFramework(entitiesToDelete);
                    }

                    transaction.Complete();
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

            using var context = _contextFactory.Create();

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

        protected IQueryable<IGrouping<long, List<long>>> CreateSearchAggregateQuery(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys)
        {
            var query = CreateSearchQuery(context, filters, includes, orderBys, null);

            var aggregateQuery = _filterService.AddAggregationFilter(query, groupBys, paging, orderBys);

            return aggregateQuery;
        }

        protected virtual long SearchResultCountOnly(List<T> filters)
        {
            using var context = _contextFactory.Create();

            var query = QueryableSearch(context, filters);

            if (query != null)
            {
                return query.Count();
            }

            return 0;
        }

        protected virtual long SearchAggregateResultCountOnly(List<T> filters, List<string> groupBys)
        {
            using var context = _contextFactory.Create();

            var searchAggregateQuery = QueryableSearchAggregate(context, filters, null, null, null, groupBys);

            if (searchAggregateQuery != null)
            {
                return searchAggregateQuery.Count();
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

                queryForFilter = queryForFilter.AsNoTracking();

                queryForFilter = _scopeOfResponsibilityService.FilterResultOnCurrentPrincipal(queryForFilter);

                queryForFilter = _filterService.FilterResultsOnSearch(queryForFilter, filter, context);

                queryForFilter = _filterService.AddIncludes(queryForFilter, includes, context);

                completeQuery = completeQuery == null ? queryForFilter : completeQuery.Concat(queryForFilter);
            }

            if (completeQuery != null)
            {
                completeQuery = completeQuery.Distinct();

                completeQuery = _filterService.AddOrderBys(completeQuery, orderBys, context);

                completeQuery = _filterService.AddPaging(completeQuery, paging, context);
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

        protected virtual void DeleteUsingEntityFramework(List<T> entitiesToDelete)
        {
            using var context = _contextFactory.Create();

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

        protected virtual void DeleteUsingBulkDelete(List<T> entitiesToDelete)
        {
            using var context = _contextFactory.Create();
            var idsToDelete = string.Join(",", entitiesToDelete.Select(x => x.GetIdFromEntity()).ToList());
            var entityTableName = typeof(T).GetTableName(context);
            var tsql = $"DELETE FROM {entityTableName} WHERE {typeof(T).GetPrimaryKeyProperty().Name} IN ({idsToDelete})";
            context.DatabaseAccessor.ExecuteSqlCommand(tsql);
        }
    }
}