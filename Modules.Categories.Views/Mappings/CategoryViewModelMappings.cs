using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;

namespace Modules.Categories.Views.Mappings;

public static class CategoryViewModelMappings
{
    public static Category Map(this CategoryViewModel vm)
    {
        return new Category
        {
            Id = vm.Id,
            Name = vm.Name,
            ListOrder = vm.ListOrder,
            CreationDate = vm.CreationDate,
            ModificationDate = vm.ModificationDate,
            IsDeleted = vm.IsDeleted,
        };
    }

    public static List<Category> MapList(this IEnumerable<CategoryViewModel> vmList) =>
        vmList.Select(x => x.Map()).ToList();

    public static CategoryViewModel MapToViewModel(this Category category)
    {
        return new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            ListOrder = category.ListOrder,
            CreationDate = category.CreationDate,
            ModificationDate = category.ModificationDate,
            IsDeleted = category.IsDeleted
        };
    }

    public static List<CategoryViewModel> MapToViewModelList(this IEnumerable<Category> categoryList) =>
        categoryList.Select(x => x.MapToViewModel()).ToList();
}
