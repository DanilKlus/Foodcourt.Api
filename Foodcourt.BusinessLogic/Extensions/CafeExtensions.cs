using Foodcourt.Data.Api.Entities.Cafes;
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
    }
}