using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using EfYou.Model.Attributes;
using EfYou.Security.Models.Enumerations;

namespace EfYou.Security.Models
{
    public class LoginAttempt
    {
        [DefaultValue(UtcNow = true)]
        public DateTime? DateTime { get; set; }

        public LoginAttemptResult? LoginAttemptResult { get; set; }

        [XmlIgnore] public virtual Login Login { get; set; }

        public int LoginId { get; set; }

        [Key] public int Id { get; set; }
    }
}
