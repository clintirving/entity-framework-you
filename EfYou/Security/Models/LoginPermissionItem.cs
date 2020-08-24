// // -----------------------------------------------------------------------
// // <copyright file="LoginPermissionItem.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace EfYou.Security.Models
{
    public class LoginPermissionItem
    {
        public int ItemId { get; set; }

        [XmlIgnore] public virtual LoginPermission LoginPermission { get; set; }

        public int LoginPermissionId { get; set; }

        [Key] public int Id { get; set; }
    }
}