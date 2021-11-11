using System;
using System.Collections.Generic;
using EfYou.Filters;

namespace EfYou.EntityServices
{
    public interface ITimescaleEntityService<T> : IEntityService<T> where T : class, new()
    {
        List<T> Get(List<DateTime> intervals);
        List<T> Get(List<DateTime> intervals, List<string> includes);
        List<T> Get(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys);
        T GetFirst(List<DateTime> intervals);
        T GetFirst(List<DateTime> intervals, List<string> includes);
        T GetFirst(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys);
        List<T> Get(List<DateTime> intervals, List<string> includes, List<OrderBy> orderBys, Paging paging);
        void DeleteChunks(string interval);
        void Delete(List<DateTime> ids);
    }
}