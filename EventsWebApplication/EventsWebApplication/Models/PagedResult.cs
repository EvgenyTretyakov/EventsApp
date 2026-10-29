namespace EventsWebApplication.Models
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int CurrentPageTotalItems { get; set; }
    }
}
