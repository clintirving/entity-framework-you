// // -----------------------------------------------------------------------
// // <copyright file="IFilterServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace EfYou.Filters
{
    public interface IFilterService<T> where T : class, new()
    {
        IQueryable<T> FilterResultsOnSearch(IQueryable<T> query, T filter);
        IQueryable<T> FilterResultsOnGet(IQueryable<T> query, List<long> ids);
        IQueryable<T> AddIncludes(IQueryable<T> query, List<string> includes);
        IQueryable<T> AddOrderBys(IQueryable<T> query, List<OrderBy> orderBys);
        IQueryable<T> AddPaging(IQueryable<T> query, Paging paging);
        IQueryable<IGrouping<long, List<long>>> AddPaging(IQueryable<IGrouping<long, List<long>>> query, Paging paging);
        IQueryable<IGrouping<long, List<long>>> AddAggregationFilter(IQueryable<T> query, List<string> groupBys, Paging paging, List<OrderBy> orderBys);
        IQueryable<T> FilterResultsOnGet(IQueryable<T> query, List<DateTime> intervals);
    }
}