using System.ComponentModel.DataAnnotations;

namespace Foodcourt.Data.Api.Request;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    public double? Proteins { get; set; }
    public double? Fats { get; set; }
    public double? Carbohydrates { get; set; }
    public double? Weight { get; set; }
    public double? Kcal { get; set; }
}