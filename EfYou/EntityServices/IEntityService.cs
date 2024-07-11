// // -----------------------------------------------------------------------
// // <copyright file="IEntityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using EfYou.DatabaseContext;
using EfYou.Filters;

namespace EfYou.EntityServices
{
    public interface IEntityService<T> where T : class, new()
    {
        /// <summary>
        /// If True DELETE skips Audit regardless of whether AuditMe is specified on the Entity and deletes directly in the DB with a SQL Command.
        /// </summary>
        bool UseBulkDelete { get; set; }
        List<T> Get(List<dynamic> ids);
        List<T> Get(List<dynamic> ids, List<string> includes);
        List<T> Get(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys);
        T GetFirst(List<dynamic> ids);
        T GetFirst(List<dynamic> ids, List<string> includes);
        T GetFirst(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys);
        List<T> Get(List<dynamic> ids, List<string> includes, List<OrderBy> orderBys, Paging paging);
        IQueryable<T> QueryableGet(IContext context, List<dynamic> ids);
        IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes);
        IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes, List<OrderBy> orderBys);
        IQueryable<T> QueryableGet(IContext context, List<dynamic> ids, List<string> includes, List<OrderBy> orderBys, Paging paging);
        List<T> Search(List<T> filters);
        List<T> Search(List<T> filters, List<string> includes);
        List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys);
        List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging);
        IQueryable<T> QueryableSearch(IContext context, List<T> filters);
        IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes);
        IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys);
        IQueryable<T> QueryableSearch(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging);
        List<List<long>> SearchAggregate(List<T> filters, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys);
        IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters);
        IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes);
        IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys);
        IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging);
        IQueryable<IGrouping<long, List<long>>> QueryableSearchAggregate(IContext context, List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys);
        T SearchFirst(List<T> filters);
        T SearchFirst(List<T> filters, List<string> includes);
        T SearchFirst(List<T> filters, List<string> includes, List<OrderBy> orderBys);
        long SearchCount(List<T> filters);
        long SearchAggregateCount(List<T> filters, List<string> groupBys);
        List<T> Add(List<T> entitiesToAdd);
        void Delete(List<object> ids);
        void Update(List<T> items);
    }
}