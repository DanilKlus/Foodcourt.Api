using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Entities.Users;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Extensions
{
    public static class BasketProductExtensions
    {
        public static BasketProductResponse ToEntity(this BasketProduct basketProduct)
        {
            var product = basketProduct.Product;
            return new BasketProductResponse()
            {
                Id = basketProduct.Id,
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Avatar = product.Avatar,
                Status = product.Status,
                Price = product.Price,
                CafeId = product.CafeId,
                Count = basketProduct.Count,
                ProductVariants = new ProductVariantResponse(basketProduct.ProductVariant.Id, basketProduct.ProductVariant.Variant),
                ProductTypes = product.ProductTypes != null
                    ? product.ProductTypes.Select(p => new ProductTypeResponse(p.Id, p.Type)).ToList()
                    : new List<ProductTypeResponse>()
            };
        }
        
        public static OrderProduct ToOrder(this BasketProduct basketProduct) =>
            new OrderProduct
            {
                Count = basketProduct.Count,
                ProductId = basketProduct.ProductId,
                ProductVariantId = basketProduct.ProductVariantId,
            };
    }
}