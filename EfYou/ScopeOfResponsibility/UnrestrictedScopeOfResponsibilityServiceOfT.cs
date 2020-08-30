// // -----------------------------------------------------------------------
// // <copyright file="UnrestrictedScopeOfResponsibility.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Linq;

namespace EfYou.ScopeOfResponsibility
{
    public class UnrestrictedScopeOfResponsibilityServiceOfT<T> : IScopeOfResponsibilityService<T> where T : class, new()
    {
        public IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query)
        {
            return query;
        }
    }
}