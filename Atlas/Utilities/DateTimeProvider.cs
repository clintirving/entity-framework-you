// // -----------------------------------------------------------------------
// // <copyright file="DateTimeProvider.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace Atlas.Utilities
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now => DateTime.Now;

        public TimeSpan GetCurrentLocalTimeForTimezone(string timeZone)
        {
            return TimeZoneInfo.ConvertTime(UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).TimeOfDay;
        }
    }
}