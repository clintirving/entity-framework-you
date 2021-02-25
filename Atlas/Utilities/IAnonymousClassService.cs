// // -----------------------------------------------------------------------
// // <copyright file="IAnonymousClassService.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Utilities
{
    public interface IAnonymousClassService
    {
        Type CreateAnonymousType(List<PropertyInfo> properties);
    }
}