﻿// // -----------------------------------------------------------------------
// // <copyright file="CollectionContains.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;

namespace EfYou.Model.FilterExtensions
{
    public class ListContains<T> : List<T>
    {
        public ListContains()
            : base() { }

        public ListContains(int capacity)
            : base(capacity) { }

        public ListContains(IEnumerable<T> collection)
            : base(collection) { }
    }
}