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
    public class Audit
    {
        [Key] public int Id { get; set; }

        public AuditAction AuditAction { get; set; }

        public DateTime DateTime { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string Email { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string Type { get; set; }

        [StringLength(36)]
        public string TypeId { get; set; }

        [Filter(AllowPartialStringMatch = true)]
        public string SerializedEntity { get; set; }
    }
}