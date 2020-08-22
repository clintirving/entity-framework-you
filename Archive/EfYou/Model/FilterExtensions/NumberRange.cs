// // -----------------------------------------------------------------------
// // <copyright file="NumberRange.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace EfYou.Model.FilterExtensions
{
    public class NumberRange
    {
        public NumberRange()
        {
            Max = double.MaxValue;
            Min = double.MinValue;
        }

        public double Max { get; set; }

        public double Min { get; set; }
    }
}