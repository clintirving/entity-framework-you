// // -----------------------------------------------------------------------
// // <copyright file="IDateTimeProvider.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace Atlas.Utilities
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        TimeSpan GetCurrentLocalTimeForTimezone(string timeZone);
    }
}