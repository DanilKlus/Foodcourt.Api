using System.ComponentModel.DataAnnotations;
using Foodcourt.Data.Api.Request;

namespace Foodcourt.Api.Validation.Attributes
{
    public class ValidPatchProductRequest : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is PatchProductRequest patchProductRequest)
            {
                if (patchProductRequest.Count is < 1)
                    return new ValidationResult("Количество продуктов должно быть больше 0", new[] { nameof(patchProductRequest.Count) });
                if (patchProductRequest.VariantId is < 1)
                    return new ValidationResult("Id варианта продукта должен  быть больше 0", new[] { nameof(patchProductRequest.VariantId) });
                
                return ValidationResult.Success;
            }
           
            return new ValidationResult("В запросе передан некорректный запрос");
        }
    }
}