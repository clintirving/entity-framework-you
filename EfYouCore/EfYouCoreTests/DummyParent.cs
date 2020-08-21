// // -----------------------------------------------------------------------
// // <copyright file="DummyParent.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EfYouCoreTests
{
    public class DummyParent
    {
        public virtual List<DummyEntity> DummyEntities { get; set; }

        [Key] public long Id { get; set; }
    }
}