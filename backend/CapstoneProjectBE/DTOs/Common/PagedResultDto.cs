namespace CapstoneProjectBE.DTOs.Common
{

    /// <summary>
    /// Carries Paged Result data between API clients and application services.
    /// </summary>
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
