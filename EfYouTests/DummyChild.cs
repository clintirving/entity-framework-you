// // -----------------------------------------------------------------------
// // <copyright file="DummyChild.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using EfYouCore.Model.Attributes;

namespace EfYouCoreTests
{
    public class DummyChild
    {
        public int DummyEntityId { get; set; }

        public virtual DummyEntity DummyEntity { get; set; }

        [DefaultValue(Value = "100")] public int? IntWithDefaultValue { get; set; }

        public int Id { get; set; }
    }
}