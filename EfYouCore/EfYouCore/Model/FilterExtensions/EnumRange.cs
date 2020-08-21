// // -----------------------------------------------------------------------
// // <copyright file="EnumRange.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace EfYouCore.Model.FilterExtensions
{
    public class EnumRange
    {
        public EnumRange()
        {
            Max = int.MaxValue;
            Min = int.MinValue;
        }

        public int Max { get; set; }

        public int Min { get; set; }
    }
}