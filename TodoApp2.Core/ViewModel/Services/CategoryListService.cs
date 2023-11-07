using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TodoApp2.Core.Extensions;
using TodoApp2.Core.Mappings;
using TodoApp2.Core.Reordering;
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
        private int _lastRemovedId = int.MinValue;

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

            _activeCategory = _context.Categories
                .First(x => x.Id == IoC.AppSettings.SessionSettings.ActiveCategoryId)
                .Map();

            if (_activeCategory == null)
            {
                _activeCategory = _context.Categories.First().Map();
            }

            var categories = _context.Categories
                .Where(x => !x.Trashed)
                .OrderByListOrder()
                .MapList();

            Items = new ObservableCollection<CategoryViewModel>(categories);
            Items.CollectionChanged += ItemsOnCollectionChanged;
        }

        public CategoryViewModel GetCategory(int id) => _context.Categories.First(x => x.Id == id).Map();

        protected override void OnDispose()
        {
            Items.CollectionChanged -= ItemsOnCollectionChanged;
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Trigger update to refresh move to category items context menu
            OnPropertyChanged(nameof(InactiveCategories));

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems.Count > 0)
                    {
                        var newItem = (CategoryViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == _lastRemovedId)
                        {
                            ReorderingHelperTemp.ReorderCategory(_context, newItem, e.NewStartingIndex);
                        }

                        _lastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        var last = (CategoryViewModel)e.OldItems[0];

                        _lastRemovedId = last.Id;
                    }
                    break;
                }
            }
        }
    }
}
