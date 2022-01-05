// // -----------------------------------------------------------------------
// // <copyright file="ICascadeDeleteServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;

namespace EfYou.CascadeDelete
{
    public interface ICascadeDeleteService<T> where T : class, new()
    {
        void CascadeDelete(List<dynamic> ids);
    }
}