using Foodcourt.Data.Api.Entities.Orders;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Extensions
{
    public static class OrderExtensions
    {
        public static OrderResponse ToEntity(this Order order) =>
            new()
            {
                Id = order.Id,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                CreationTime = order.CreationTime,
                Comment = order.Comment,
                CafeId = order.CafeId,
                CafeName = order.CafeName,
                Products = order.OrderProducts != null ? order.OrderProducts.Select(x => x.ToEntity()).ToList() : new List<OrderProductResponse>()
            };
        
        public static OrderProductResponse ToEntity(this OrderProduct orderProduct)
        {
            var product = orderProduct.Product;
            return new OrderProductResponse()
            {
                Id = orderProduct.Id,
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Avatar = product.Avatar,
                Status = product.Status,
                Price = product.Price,
                CafeId = product.CafeId,
                Count = orderProduct.Count,
                ProductVariants = new ProductVariantResponse(orderProduct.ProductVariant.Id, orderProduct.ProductVariant.Variant),
                ProductTypes = product.ProductTypes != null
                    ? product.ProductTypes.Select(p => new ProductTypeResponse(p.Id, p.Type)).ToList()
                    : new List<ProductTypeResponse>()
            };
        }
    }
    
    
}