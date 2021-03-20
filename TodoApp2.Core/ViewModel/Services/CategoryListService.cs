using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the category list and the currently selected category.
    /// Because this is a service, it can be accessed from multiple viewmodels.
    /// </summary>
    public class CategoryListService : BaseViewModel
    {
        /// <summary>
        /// The category list items
        /// </summary>
        public ObservableCollection<CategoryListItemViewModel> Items { get; set; }

        /// <summary>
        /// The currently selected category
        /// </summary>
        public string CurrentCategory { get; set; }

        public CategoryListService()
        {
            List<CategoryListItemViewModel> categories = IoC.ClientDatabase.GetActiveCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);
        }
    }
}
