using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EfYouTests
{
    public class DummyEntityWithGuidId
    {
        [Key]
        public Guid Id { get; set; }
    }
}
