using System.Collections.Generic;
using System.Linq;
using TodoApp2.Common;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Mappings;

public static class CategoryMapping
{
    public static Category Map(this CategoryViewModel vm)
    {
        if (vm == null) return null;

        return new Category
        {
            Id = vm.Id,
            Name = vm.Name,
            ListOrder = vm.ListOrder.FormatListOrder(),
            Trashed = vm.Trashed
        };
    }

    public static CategoryViewModel Map(this Category category)
    {
        if (category == null) return null;

        return new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            ListOrder = ListOrderParsingHelper.ParseListOrder(category.ListOrder),
            Trashed = category.Trashed
        };
    }

    public static List<Category> MapList(this IEnumerable<CategoryViewModel> vmList) => 
        vmList.Select(x => x.Map()).ToList();

    public static List<CategoryViewModel> MapList(this IEnumerable<Category> modelList) => 
        modelList.Select(x => x.Map()).ToList();
}
