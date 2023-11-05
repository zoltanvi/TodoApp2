using System.Collections.Generic;
using System.Linq;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Mappings
{
    public static class CategoryMapping
    {
        public static Category Map(this CategoryViewModel vm)
        {
            if (vm == null) return null;

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
            if (category == null) return null;

            return new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ListOrder = long.Parse(category.ListOrder),
                Trashed = category.Trashed
            };
        }

        public static List<Category> Map(this IEnumerable<CategoryViewModel> vmList) => 
            vmList.Select(x => x.Map()).ToList();

        public static List<CategoryViewModel> Map(this IEnumerable<Category> modelList) => 
            modelList.Select(x => x.Map()).ToList();
    }
}
