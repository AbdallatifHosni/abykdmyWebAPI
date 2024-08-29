namespace AbyKhedma.Pagination
{
    public class PaginationCalculator
    {
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public int PageNumber { get; set; }

        public PaginationCalculator(int pageSize, int rowCount, int pageNumber)
        {
            PageSize = pageSize;
            RowCount = rowCount;
            PageNumber = pageNumber;
        }

        public int CalculateTotalPages()
        {
            return (int)Math.Ceiling((double)RowCount / PageSize);
        }

        public int CalculateStartIndex()
        {
            return (PageNumber - 1) * PageSize;
        }

        public int CalculateEndIndex()
        {
            return Math.Min(CalculateStartIndex() + PageSize - 1, RowCount - 1);
        }
    }

}
