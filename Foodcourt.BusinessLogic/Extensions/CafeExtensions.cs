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
                Adress = cafe.Adress,
                Rating = cafe.Rating
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
                Adress = cafe.Adress,
                Distance = stringDist,
                Rating = cafe.Rating
            };
        }
        
        public static Cafe FromEntity(this CafeCreateRequest cafe)
        {
            return new Cafe()
            {
                Name = cafe.Name,
                Description = cafe.Description,
                Adress = cafe.Adress,
                Avatar = "",
                Response = "",
                Latitude = cafe.Latitude,
                Longitude = cafe.Longitude,
                CertifyingDocument = cafe.CertifyingDocument,
                PersonalAccount = cafe.PersonalAccount
            };
        }
    }
}