// // -----------------------------------------------------------------------
// // <copyright file="FilterExtensionsAttribute.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfYou.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterExtensionsAttribute : NotMappedAttribute
    {
        public string AppliedToProperty { get; set; }
    }
}