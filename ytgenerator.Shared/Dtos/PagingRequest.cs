namespace ytgenerator.Shared.Dtos
{
    public class PagingRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PagingRequest()
        {
            PageIndex = 0;
            PageSize = 50;
        }
    }
}
