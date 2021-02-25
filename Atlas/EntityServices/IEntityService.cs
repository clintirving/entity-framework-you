// // -----------------------------------------------------------------------
// // <copyright file="IEntityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;
using Atlas.Filters;

namespace Atlas.EntityServices
{
    public interface IEntityService<T> where T : class, new()
    {
        List<T> Get(List<long> ids);
        List<T> Get(List<long> ids, List<string> includes);
        List<T> Get(List<long> ids, List<string> includes, List<OrderBy> orderBys);
        List<T> Get(List<long> ids, List<string> includes, List<OrderBy> orderBys, Paging paging);
        T GetFirst(List<long> ids);
        T GetFirst(List<long> ids, List<string> includes);
        T GetFirst(List<long> ids, List<string> includes, List<OrderBy> orderBys);
        List<T> Search(List<T> filters);
        List<T> Search(List<T> filters, List<string> includes);
        List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys);
        List<T> Search(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging);
        T SearchFirst(List<T> filters);
        T SearchFirst(List<T> filters, List<string> includes);
        T SearchFirst(List<T> filters, List<string> includes, List<OrderBy> orderBys);
        long SearchCount(List<T> filters);
        List<T> Add(List<T> entitiesToAdd);
        void Delete(List<long> ids);
        void Update(List<T> items);
        List<List<long>> SearchAggregate(List<T> filters, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, List<string> aggregate);
        List<List<long>> SearchAggregate(List<T> filters, List<string> includes, List<OrderBy> orderBys, Paging paging, List<string> groupBys);
        long SearchAggregateCount(List<T> filters, List<string> groupBys);
    }
}