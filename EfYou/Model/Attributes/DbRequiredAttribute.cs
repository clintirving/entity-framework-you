using System.ComponentModel.DataAnnotations;

namespace EfYou.Model.Attributes
{
    public class DbRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return null;
        }
    }
}
