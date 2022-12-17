using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
        private CategoryListItemViewModel m_ActiveCategory;

        /// <summary>
        /// The category list items
        /// </summary>
        public ObservableCollection<CategoryListItemViewModel> Items { get; set; }

        public IEnumerable<CategoryListItemViewModel> InactiveCategories => Items.Where(c => c.Id != ActiveCategory?.Id);

        /// <summary>
        /// The name of the currently selected category
        /// </summary>
        public string ActiveCategoryName
        {
            get => ActiveCategory?.Name;

            set
            {
                if (!string.IsNullOrWhiteSpace(value) && ActiveCategory.Name != value)
                {
                    ActiveCategory.Name = value;
                    m_Database.UpdateCategory(ActiveCategory);
                }
            }
        }

        public CategoryListItemViewModel ActiveCategory
        {
            get => m_ActiveCategory;
            set
            {
                m_ActiveCategory = value;
                m_ApplicationViewModel.ApplicationSettings.ActiveCategoryId = value?.Id ?? -1;
            } 
        }

        public CategoryListService(ApplicationViewModel applicationViewModel, IDatabase database)
        {
            m_ApplicationViewModel = applicationViewModel;
            m_Database = database;
            m_ActiveCategory = m_Database.GetCategory(m_ApplicationViewModel.ApplicationSettings.ActiveCategoryId);

            m_Database.CategoryChanged += OnDatabaseCategoryChanged;

            List<CategoryListItemViewModel> categories = m_Database.GetValidCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);
            Items.CollectionChanged += OnItemsChanged;
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Trigger update to refresh move to category items context menu
            OnPropertyChanged(nameof(InactiveCategories));
        }

        private void OnDatabaseCategoryChanged(object sender, CategoryChangedEventArgs e)
        {
            // Update the category in the app settings
            ActiveCategory = e.ChangedCategory;

            // Update the category in the items list
            CategoryListItemViewModel modifiedItem = Items.FirstOrDefault(item => item.Id == e.ChangedCategory.Id);
            modifiedItem.Name = e.ChangedCategory.Name;
            m_ApplicationViewModel.SaveApplicationSettings();
        }

        public CategoryListItemViewModel GetCategory(int id) => m_Database.GetCategory(id);

        public CategoryListItemViewModel GetCategory(string categoryName) => m_Database.GetCategory(categoryName);

        private void OnOnlineModeChanged(object obj)
        {
            Items.Clear();
            List<CategoryListItemViewModel> categories = m_Database.GetValidCategories();
            Items.AddRange(categories);
            OnPropertyChanged(nameof(ActiveCategory));
        }

        protected override void OnDispose()
        {
            Items.CollectionChanged -= OnItemsChanged;
        }
    }
}
