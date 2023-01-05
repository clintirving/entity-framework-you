// // -----------------------------------------------------------------------
// // <copyright file="LoginPermission.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using EfYou.Model.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace EfYou.Security.Models
{
    public class LoginPermission
    {
        [DbRequired(AllowEmptyStrings = false)] public bool? FullAccess { get; set; }

        [XmlIgnore] public virtual List<LoginPermissionItem> LoginPermissionItems { get; set; }

        [DbRequired(AllowEmptyStrings = false)] public string Type { get; set; }

        [XmlIgnore] public virtual Login Login { get; set; }

        public int LoginId { get; set; }

        [Key] public int Id { get; set; }
    }
}