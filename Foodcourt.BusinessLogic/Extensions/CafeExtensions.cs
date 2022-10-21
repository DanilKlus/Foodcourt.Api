using Foodcourt.Data.Api.Entities.Cafes;
using Foodcourt.Data.Api.Response;

namespace Foodcourt.BusinessLogic.Extensions
{
    public static class CafeExtensions
    {
        public static CafeSearchResponse ToEntity(this Cafe cafe)
        {
            return new CafeSearchResponse()
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Description = cafe.Description,
                Status = cafe.Status,
                Avatar = cafe.Avatar,
                Location = cafe.Location
            };
        }
    }
}