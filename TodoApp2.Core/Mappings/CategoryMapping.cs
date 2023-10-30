using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Mappings
{
    internal static class CategoryMapping
    {
        public static Category Map(this CategoryViewModel vm)
        {
            return new Category
            {
                Id = vm.Id,
                Name = vm.Name,
                ListOrder = vm.ListOrder.ToString(GlobalConstants.ListOrderFormat),
                Trashed = vm.Trashed
            };
        }

        public static CategoryViewModel Map(this Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ListOrder = long.Parse(category.ListOrder),
                Trashed = category.Trashed
            };
        }

    }
}
