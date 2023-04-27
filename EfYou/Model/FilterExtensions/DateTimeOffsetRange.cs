// // -----------------------------------------------------------------------
// // <copyright file="DateTimeRange.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace EfYou.Model.FilterExtensions
{
    public class DateTimeOffsetRange
    {
        public DateTimeOffsetRange()
        {
            After = DateTimeOffset.MinValue;
            Before = DateTimeOffset.MaxValue;
        }

        public DateTimeOffset After { get; set; }
        public DateTimeOffset Before { get; set; }
    }
}