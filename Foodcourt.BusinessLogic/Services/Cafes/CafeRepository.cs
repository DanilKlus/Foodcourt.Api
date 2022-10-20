using Foodcourt.BusinessLogic.Extensions;
using Foodcourt.Data;
using Foodcourt.Data.Api;
using Foodcourt.Data.Api.Request;
using Foodcourt.Data.Entities.Cafes;
using Foodcourt.Data.Entities.Response;
using Microsoft.EntityFrameworkCore;

namespace Foodcourt.BusinessLogic.Services.Cafes;

public class CafeRepository : ICafeRepository
{
    private readonly DataContext dataContext;

    public CafeRepository(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }
    
    public async Task<List<Cafe>> Search(CafeSearchRequest search) => 
        await dataContext.Cafes.ToListAsync();

    public async Task<Cafe?> Get(Guid cafeId)
    {
        var cafe = await dataContext.Cafes.Where(cafe => cafe.Id.Equals(cafeId)).FirstOrDefaultAsync();
        return cafe;
    }
}