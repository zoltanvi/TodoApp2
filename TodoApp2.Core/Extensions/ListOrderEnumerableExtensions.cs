using System.Collections.Generic;
using System.Linq;
using TodoApp2.Core.Helpers;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Extensions;

internal static class ListOrderEnumerableExtensions
{
    public static IEnumerable<Task> OrderByListOrder(this IEnumerable<Task> source)
    {
        return source.OrderBy(x => x.ListOrder, new NumericStringComparer());
    }

    public static IEnumerable<Task> OrderByDescendingListOrder(this IEnumerable<Task> source)
    {
        return source.OrderByDescending(x => x.ListOrder, new NumericStringComparer());
    }

    public static IEnumerable<Category> OrderByListOrder(this IEnumerable<Category> source)
    {
        return source.OrderBy(x => x.ListOrder, new NumericStringComparer());
    }

    public static IEnumerable<Category> OrderByDescendingListOrder(this IEnumerable<Category> source)
    {
        return source.OrderByDescending(x => x.ListOrder, new NumericStringComparer());
    }
}
