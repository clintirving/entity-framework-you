// // -----------------------------------------------------------------------
// // <copyright file="OrderBy.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace Atlas.Filters
{
    public class OrderBy
    {
        public string ColumnName { get; set; }

        public bool Descending { get; set; }
    }
}