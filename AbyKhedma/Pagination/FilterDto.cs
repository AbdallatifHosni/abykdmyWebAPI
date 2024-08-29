namespace AbyKhedma.Pagination
{
    public class FilterDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FilterDto()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public FilterDto(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize ==0 ? 10 : pageSize;
        }
    }
}
