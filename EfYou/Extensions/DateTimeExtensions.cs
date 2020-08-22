// // -----------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace EfYouCore.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToTimezone(this DateTime dateTime, string timezone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(timezone));
        }
    }
}