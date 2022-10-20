using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Entities.Cafes;
using Foodcourt.Data.Entities.Response;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public interface ICafeRepository
{
    Task<List<Cafe>> Search(CafeSearchRequest search);
    Task<Cafe?> Get(Guid cafeId);

}