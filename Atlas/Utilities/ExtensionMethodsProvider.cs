// // -----------------------------------------------------------------------
// // <copyright file="ExtensionMethodsProvider.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace Atlas.Utilities
{
    public static class ExtensionMethodsProvider
    {
        public static bool IsTimeBetween(this DateTime datetime, TimeSpan start, TimeSpan end)
        {
            var now = datetime.TimeOfDay;

            return start < end ? start <= now && now <= end : !(end < now && now < start);
        }
    }
}