using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the category list and the currently selected category.
    /// Because this is a service, it can be accessed from multiple ViewModels.
    /// </summary>
    public class CategoryListService : BaseViewModel
    {
        private readonly ApplicationViewModel m_ApplicationViewModel;
        private readonly IDatabase m_Database;

        /// <summary>
        /// The category list items
        /// </summary>
        public ObservableCollection<CategoryListItemViewModel> Items { get; set; }

        /// <summary>
        /// The currently selected category
        /// </summary>
        public string CurrentCategory
        {
            get => m_ApplicationViewModel.ApplicationSettings.CurrentCategory;
            set => m_ApplicationViewModel.ApplicationSettings.CurrentCategory = value;
        }

        public CategoryListService(ApplicationViewModel applicationViewModel, IDatabase database)
        {
            m_ApplicationViewModel = applicationViewModel;
            m_Database = database;

            Mediator.Register(OnOnlineModeChanged, ViewModelMessages.OnlineModeChanged);

            List<CategoryListItemViewModel> categories = m_Database.GetActiveCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);
        }
        
        public CategoryListItemViewModel GetCurrentCategory => m_Database.GetCategory(CurrentCategory);
        
        public CategoryListItemViewModel GetCategory(int id) => m_Database.GetCategory(id);
        public CategoryListItemViewModel GetCategory(string categoryName) => m_Database.GetCategory(categoryName);

        private void OnOnlineModeChanged(object obj)
        {
            Items.Clear();
            List<CategoryListItemViewModel> categories = m_Database.GetActiveCategories();
            Items.AddRange(categories);
            OnPropertyChanged(nameof(CurrentCategory));
        }
    }
}
