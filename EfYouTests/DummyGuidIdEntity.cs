using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EfYouTests
{
    public class DummyGuidIdEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
