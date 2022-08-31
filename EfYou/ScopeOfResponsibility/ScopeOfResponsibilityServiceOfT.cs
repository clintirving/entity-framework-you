// // -----------------------------------------------------------------------
// // <copyright file="ScopeOfResponsibilityService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using EfYou.Extensions;

namespace EfYou.ScopeOfResponsibility
{
    public abstract class ScopeOfResponsibilityServiceOfT<T> : IScopeOfResponsibilityService<T> where T : class, new()
    {
        public virtual IQueryable<T> FilterResultOnCurrentPrincipal(IQueryable<T> query)
        {
            if (RestrictScopeOfResponsibilityOnLoginConfiguration(out var restrictedToIds))
            {
                var primaryKeyProperty = typeof(T).GetPrimaryKeyProperty();

                if (primaryKeyProperty.PropertyType != typeof(int))
                {
                    throw new ApplicationException(
                        $"Primary Key property of {primaryKeyProperty.PropertyType} is not valid for filtering on scope of responsibility, it must be an int");
                }

                return query.Where($"{primaryKeyProperty.Name} in @0", restrictedToIds);
            }

            return query;
        }

        public abstract bool RestrictScopeOfResponsibilityOnLoginConfiguration(out List<int> ids);

    }
}