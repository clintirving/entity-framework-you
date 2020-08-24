// // -----------------------------------------------------------------------
// // <copyright file="DummyChild.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using EfYou.Model.Attributes;

namespace EfYouTests
{
    public class DummyChild
    {
        public int DummyEntityId { get; set; }

        public virtual DummyEntity DummyEntity { get; set; }

        [DefaultValue(Value = "100")] public int? IntWithDefaultValue { get; set; }

        [Key]
        public short Id { get; set; }
    }
}