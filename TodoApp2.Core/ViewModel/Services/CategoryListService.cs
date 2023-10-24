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
        private readonly AppViewModel _appViewModel;
        private readonly IDatabase _database;
        private CategoryViewModel _activeCategory;

        /// <summary>
        /// The category list items
        /// </summary>
        public ObservableCollection<CategoryViewModel> Items { get; set; }

        public IEnumerable<CategoryViewModel> InactiveCategories => Items.Where(c => c.Id != ActiveCategory?.Id);

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
                    _database.UpdateCategory(ActiveCategory);
                }
            }
        }

        public CategoryViewModel ActiveCategory
        {
            get => _activeCategory;
            set
            {
                _activeCategory = value;
                IoC.AppSettings.SessionSettings.ActiveCategoryId = value?.Id ?? -1;
            }
        }

        public CategoryListService(AppViewModel applicationViewModel, IDatabase database)
        {
            _appViewModel = applicationViewModel;
            _database = database;
            _activeCategory = _database.GetCategory(IoC.AppSettings.SessionSettings.ActiveCategoryId);

            _database.CategoryChanged += OnDatabaseCategoryChanged;

            List<CategoryViewModel> categories = _database.GetValidCategories();
            Items = new ObservableCollection<CategoryViewModel>(categories);
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
            CategoryViewModel modifiedItem = Items.FirstOrDefault(item => item.Id == e.ChangedCategory.Id);
            modifiedItem.Name = e.ChangedCategory.Name;
            _appViewModel.SaveApplicationSettings();
        }

        public CategoryViewModel GetCategory(int id) => _database.GetCategory(id);

        protected override void OnDispose()
        {
            Items.CollectionChanged -= OnItemsChanged;
        }
    }
}
