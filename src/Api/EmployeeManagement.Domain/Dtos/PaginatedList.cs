using System;
using System.Collections.Generic;

namespace EmployeeManagement.Domain.Dtos
{
    public class PaginatedList<T>
        where T : class
    {
        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, long count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;
            PageItemsStartsAt = count > 0 ? ((pageIndex - 1) * pageSize) + 1 : 0;

            PageItemsEndsAt = 0;
            if (count > 0)
            {
                if (pageIndex * pageSize > count)
                {
                    PageItemsEndsAt = count;
                }
                else
                {
                    PageItemsEndsAt = pageIndex * pageSize;
                }
            }

            Items.AddRange(items);
        }

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
