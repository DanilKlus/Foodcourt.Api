namespace Foodcourt.Data.Api
{
    public class SearchResponse<TEntity>
    {
        public SearchResponse()
        {
            FoundEntities = new List<TEntity>();
            TotalCount = 0;
        }

        public SearchResponse(List<TEntity> foundEntities, long totalCount)
        {
            FoundEntities = foundEntities;
            TotalCount = totalCount;
        }

        public List<TEntity> FoundEntities { get; set; }

        public long TotalCount { get; set; }
    }
}