// // -----------------------------------------------------------------------
// // <copyright file="Audit.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using Atlas.Model.Attributes;
using Atlas.Model.Enumerations;

namespace Atlas.Model.Models
{
    public class Audit
    {
        [Key] public int Id { get; set; }

        public AuditAction AuditAction { get; set; }

        public DateTime DateTime { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string Email { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string Type { get; set; }

        public int TypeId { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string SerializedEntity { get; set; }
    }
}