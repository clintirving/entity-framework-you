// // -----------------------------------------------------------------------
// // <copyright file="CascadeDeleteServiceOfT.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;

namespace EfYou.CascadeDelete
{
    public class CascadeDeleteService<T> : ICascadeDeleteService<T> where T : class, new()
    {
        public virtual void CascadeDelete(List<long> ids)
        {
            // override this method with any cascade delete logic for the specific type.
        }
    }
}