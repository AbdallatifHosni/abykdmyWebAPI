namespace AbyKhedma.Pagination
{
    public interface IUriService
    {
        public Uri GetPageUri(FilterDto filter, string route);
    }
}
