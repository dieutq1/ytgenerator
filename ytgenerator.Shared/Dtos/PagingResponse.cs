 namespace ytgenerator.Shared.Dtos
{
    public class PagingResponse<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<T> Items { get; set; } = [];
    }
}
