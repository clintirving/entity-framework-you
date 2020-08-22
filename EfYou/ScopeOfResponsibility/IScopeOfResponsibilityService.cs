// // -----------------------------------------------------------------------
// // <copyright file="IScopeOfResponsibilityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Linq;

namespace EfYouCore.ScopeOfResponsibility
{
    public interface IScopeOfResponsibilityService<T> where T : class, new()
    {
        IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query);
    }
}