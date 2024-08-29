namespace Core.Dtos
{
    public class PagedDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PagedDto()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PagedDto(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
