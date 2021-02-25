using System;
using System.ComponentModel.DataAnnotations;

namespace AtlasTests
{
    public class DummyEntityWithGuidId
    {
        [Key]
        public Guid Id { get; set; }
    }
}
