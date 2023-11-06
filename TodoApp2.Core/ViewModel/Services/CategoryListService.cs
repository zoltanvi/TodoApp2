using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    /// <summary>
    /// Service to hold the category list and the currently selected category.
    /// </summary>
    public class CategoryListService : BaseViewModel
    {
        private readonly AppViewModel _appViewModel;
        private readonly IAppContext _context;
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
                    _context.Categories.UpdateFirst(ActiveCategory.Map());
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
                _appViewModel.SaveApplicationSettings();
            }
        }

        public CategoryListService(AppViewModel applicationViewModel, IAppContext context)
        {
            _appViewModel = applicationViewModel;
            _context = context;

            _activeCategory = _context.Categories.First(x => x.Id == IoC.AppSettings.SessionSettings.ActiveCategoryId).Map();
            if (_activeCategory == null)
            {
                _activeCategory = _context.Categories.First().Map();
            }

            var categories = _context.Categories.Where(x => !x.Trashed).OrderBy(x => x.ListOrder).Map();
            Items = new ObservableCollection<CategoryViewModel>(categories);
            Items.CollectionChanged += OnItemsChanged;
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Trigger update to refresh move to category items context menu
            OnPropertyChanged(nameof(InactiveCategories));
        }

        public CategoryViewModel GetCategory(int id) => _context.Categories.First(x => x.Id == id).Map();

        protected override void OnDispose()
        {
            Items.CollectionChanged -= OnItemsChanged;
        }
    }
}
