// // -----------------------------------------------------------------------
// // <copyright file="IPermissionServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;

namespace EfYou.Permissions
{
    public interface IPermissionService<T>
    {
        void Get();

        void Search();

        void Add();

        void Delete();

        void Update();

        bool IsInRoles(List<string> roles);
    }
}