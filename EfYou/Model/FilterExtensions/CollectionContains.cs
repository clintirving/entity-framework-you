// // -----------------------------------------------------------------------
// // <copyright file="CollectionContains.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EfYou.Model.FilterExtensions
{
    public class CollectionContains<T> : Collection<T>
    {
        public CollectionContains()
            : base() { }

        public CollectionContains(IList<T> list)
            : base(list) {}
    }
}