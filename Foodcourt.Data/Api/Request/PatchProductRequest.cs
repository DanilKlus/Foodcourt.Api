using System.ComponentModel.DataAnnotations;

namespace Foodcourt.Data.Api.Request;

public class PatchProductRequest
{
    public long? VariantId { get; set; }
    public int? Count { get; set; }
}