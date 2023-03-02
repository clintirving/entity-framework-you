// // -----------------------------------------------------------------------
// // <copyright file="IFilterServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using EfYou.DatabaseContext;
using System.Collections.Generic;
using System.Linq;

namespace EfYou.Filters
{
    public interface IFilterService<T> where T : class, new()
    {
        IQueryable<T> FilterResultsOnSearch(IQueryable<T> query, T filter, IContext context);
        IQueryable<T> FilterResultsOnGet(IQueryable<T> query, List<dynamic> ids, IContext context);
        IQueryable<T> AddIncludes(IQueryable<T> query, List<string> includes, IContext context);
        IQueryable<T> AddOrderBys(IQueryable<T> query, List<OrderBy> orderBys, IContext context);
        IQueryable<T> AddPaging(IQueryable<T> query, Paging paging, IContext context);
        IQueryable<IGrouping<long, List<long>>> AddPaging(IQueryable<IGrouping<long, List<long>>> query, Paging paging);
        IQueryable<IGrouping<long, List<long>>> AddAggregationFilter(IQueryable<T> query, List<string> groupBys, Paging paging, List<OrderBy> orderBys);
    }
}