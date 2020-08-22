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
    public class DateTimeRange
    {
        public DateTimeRange()
        {
            After = DateTime.MinValue;
            Before = DateTime.MaxValue;
        }

        public DateTime After { get; set; }
        public DateTime Before { get; set; }
    }
}