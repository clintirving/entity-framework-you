// // -----------------------------------------------------------------------
// // <copyright file="LoginReset.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace EfYouCore.Security.Models
{
    public class LoginReset
    {
        public string Code { get; set; }

        public DateTime? CodeDateTime { get; set; }

        public int Attempts { get; set; }

        [XmlIgnore] public virtual Login Login { get; set; }

        [Key] [ForeignKey("Login")] public int Id { get; set; }
    }
}