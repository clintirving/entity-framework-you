// // -----------------------------------------------------------------------
// // <copyright file="DefaultValueAttribute.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public bool UtcNow { get; set; }

        public string Value { get; set; }
    }
}