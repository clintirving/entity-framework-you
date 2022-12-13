// // -----------------------------------------------------------------------
// // <copyright file="FilterAttribute.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        public bool AllowPartialStringMatch { get; set; }

        public bool ForceCaseInsensitiveMatch { get; set; }
    }
}