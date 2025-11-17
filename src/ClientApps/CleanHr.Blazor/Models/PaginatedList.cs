using System.Collections.Generic;

namespace CleanHr.Blazor.Models;

public class PaginatedList<T>
    where T : class
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public long TotalItems { get; set; }

    public ICollection<T> Items { get; set; } = new List<T>();
}
