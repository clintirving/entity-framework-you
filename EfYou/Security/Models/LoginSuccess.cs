using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using EfYou.Model.Attributes;

namespace EfYou.Security.Models
{
    public class LoginSuccess
    {
        [DefaultValue(UtcNow = true)]
        public DateTime? DateTime { get; set; }

        [XmlIgnore] public virtual Login Login { get; set; }

        public int LoginId { get; set; }

        [Key] public int Id { get; set; }
    }
}
