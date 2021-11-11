// // -----------------------------------------------------------------------
// // <copyright file="Audit.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using EfYou.Model.Attributes;
using EfYou.Model.Enumerations;

namespace EfYou.Model.Models
{
    [Hypertable(Name = "audits")]
    public class Audit
    {
        public int AuditSerial { get; set; }

        public AuditAction AuditAction { get; set; }

        [Key]
        [HypertablePrimaryKey]
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