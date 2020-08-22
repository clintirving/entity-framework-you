using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EfYouTests
{
    public class DummyEntityWithInvalidIdType
    {
        [Key]
        public bool Id { get; set; }
    }
}
