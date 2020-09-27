using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
            where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            long count = await source.LongCountAsync();
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            List<T> items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
