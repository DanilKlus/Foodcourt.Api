using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Extensions
{
    public static class CafeExtensions
    {
        public static CafeResponse ToEntity(this Cafe cafe)
        {
            return new CafeResponse()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Avatar = cafe.Avatar,
                Address = cafe.Address,
                Rating = cafe.Rating,
                Description = cafe.Description
            };
        }
        
        public static CafeResponse ToEntity(this Cafe cafe, double distance)
        {
            var dist = Math.Round(distance);
            var stringDist = dist < 1000 ? $"{(int)dist} м" : $"{Math.Round(dist/1000, 1)} км";
            return new CafeResponse()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Avatar = cafe.Avatar,
                Address = cafe.Address,
                Distance = stringDist,
                Rating = cafe.Rating
            };
        }
        
        public static CafeApplicationResponse ToApplicationEntity(this Cafe cafe)
        {
            return new CafeApplicationResponse()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Avatar = cafe.Avatar,
                Address = cafe.Address,
                CreationTime = cafe.CreationTime
            };
        }
        
        public static FullApplicationEntity ToFullApplicationEntity(this Cafe cafe)
        {
            return new FullApplicationEntity()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Address = cafe.Address,
                CreationTime = cafe.CreationTime,
                Response = cafe.Response,
                Status = cafe.Status,
                CertifyingDocument = cafe.CertifyingDocument,
                PersonalAccount = cafe.PersonalAccount,
                Rating = cafe.Rating,
                Description = cafe.Description
            };
        }
        
        public static Cafe FromEntity(this CafeCreateRequest cafe)
        {
            return new Cafe()
            {
                Name = cafe.Name,
                Address = cafe.Address,
                Avatar = "",
                Response = "",
                Description = cafe.Description,
                Rating = cafe.Rating,
                Latitude = cafe.Latitude,
                Longitude = cafe.Longitude,
                CertifyingDocument = cafe.CertifyingDocument,
                PersonalAccount = cafe.PersonalAccount,
                CreationTime = DateTime.UtcNow
            };
        }
        
        public static SearchResponse ToSearchResponse(this Cafe cafe, double distance, List<Product> products)
        {
            var dist = Math.Round(distance);
            var stringDist = dist < 1000 ? $"{(int)dist} м" : $"{Math.Round(dist/1000, 1)} км";
            var response =  new SearchResponse()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Avatar = cafe.Avatar,
                Distance = stringDist,
                Products = new List<SearchProductResponse>()
            };
            foreach (var product in products)
            {
                var productResponse = new SearchProductResponse()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Avatar = product.Avatar,
                    Weight = product.Weight,
                    Price = product.Price
                };
                response.Products.Add(productResponse);
            }

            return response;
        }
    }
}