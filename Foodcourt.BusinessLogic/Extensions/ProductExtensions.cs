using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Extensions
{
    public static class ProductExtensions
    {
        
        public static ProductResponse ToEntity(this Product product)
        {
            return new ProductResponse()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Avatar = product.Avatar,
                Status = product.Status,
                Price = product.Price,
                CafeId = product.CafeId,
                Proteins = product.Proteins,
                Fats = product.Fats,
                Carbohydrates = product.Carbohydrates,
                Weight = product.Weight,
                Kcal = product.Kcal,
                ProductVariants = product.ProductVariants != null 
                    ? product.ProductVariants.Select(p => new ProductVariantResponse(p.Id, p.Variant)).ToList()
                    : new List<ProductVariantResponse>(),
                ProductTypes = product.ProductTypes != null
                    ? product.ProductTypes.Select(p => new ProductTypeResponse(p.Id, p.Type)).ToList()
                    : new List<ProductTypeResponse>()
            };
        }
        
        public static Product ToDbEntity(this CreateProductRequest request, Cafe cafe)
        {
            return new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Avatar = "",
                Status = ProductStatus.Available,
                Price = request.Price,
                Cafe = cafe,
                CafeId = cafe.Id,
                Proteins = request.Proteins,
                Fats = request.Fats,
                Carbohydrates = request.Carbohydrates,
                Weight = request.Weight,
                Kcal = request.Kcal,
            };
        }
    }
}