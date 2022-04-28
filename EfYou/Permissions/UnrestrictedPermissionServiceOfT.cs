// // -----------------------------------------------------------------------
// // <copyright file="PermissionServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;

namespace EfYou.Permissions
{
    public class UnrestrictedPermissionServiceOfT<T> : IPermissionService<T>
    {
        public virtual void Get()
        {
        }

        public virtual void Search()
        {
        }

        public virtual void Add()
        {
        }

        public virtual void Delete()
        {
        }

        public virtual void Update()
        {
        }

        public virtual bool IsInRoles(List<string> roles)
        {
            return true;
        }
    }
}