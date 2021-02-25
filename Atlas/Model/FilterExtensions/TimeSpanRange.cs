// // -----------------------------------------------------------------------
// // <copyright file="TimeSpanRange.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace Atlas.Model.FilterExtensions
{
    public class TimeSpanRange
    {
        public TimeSpanRange()
        {
            Max = TimeSpan.Parse("23:59:59.9999999");
            Min = TimeSpan.Parse("00:00:00.0000000");
        }

        public TimeSpan Max { get; set; }

        public TimeSpan Min { get; set; }
    }
}