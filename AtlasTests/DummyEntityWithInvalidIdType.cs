using System.ComponentModel.DataAnnotations;

namespace AtlasTests
{
    public class DummyEntityWithInvalidIdType
    {
        [Key]
        public bool Id { get; set; }
    }
}
