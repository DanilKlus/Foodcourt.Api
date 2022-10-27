using System.ComponentModel.DataAnnotations;

namespace Foodcourt.Data.Api.Request;

public class AddProductRequest
{
    [Required]
    public long Id { get; set; }
    public long? VariantId { get; set; }
}