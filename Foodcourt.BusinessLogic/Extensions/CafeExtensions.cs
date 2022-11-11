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
                Description = cafe.Description,
                Status = cafe.Status,
                Avatar = cafe.Avatar,
                Adress = cafe.Adress,
                Latitude = cafe.Latitude,
                Longitude = cafe.Longitude
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