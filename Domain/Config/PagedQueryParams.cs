namespace Domain.Config
{
    public class PagedQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public const int MaxPageSize = 100;

        public void Normalize()
        {
            if (Page <= 0) Page = 1;
            if (PageSize <= 0) PageSize = 10;
            if (PageSize > MaxPageSize) PageSize = MaxPageSize;
        }
    }
}
