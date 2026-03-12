// // -----------------------------------------------------------------------
// // <copyright file="IScopeOfResponsibilityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Linq;
using EfYou.DatabaseContext;

namespace EfYou.ScopeOfResponsibility
{
    public interface IScopeOfResponsibilityService<T> where T : class, new()
    {
        IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query);

        IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query, IContext context);
    }
}