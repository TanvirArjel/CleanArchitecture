using System.Collections.Generic;

namespace BlazorApp.Utils
{
    public class PaginatedList<T>
        where T : class
    {
        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public long TotalItems { get; set; }

        public int MaxPageLink { get; } = 5;

        public long PageItemsStartsAt { get; set; }

        public long PageItemsEndsAt { get; set; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public List<T> Items { get; set; } = new List<T>();
    }
}
