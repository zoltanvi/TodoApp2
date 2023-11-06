using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;
using TodoApp2.Core.Reordering;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    public class CategoryPageViewModel : BaseViewModel
    {
        private readonly IAppContext _context;
        private readonly AppViewModel _application;
        private readonly OverlayPageService _overlayPageService;
        private readonly CategoryListService _categoryListService;
        private readonly MessageService _messageService;

        private int _lastRemovedId = int.MinValue;

        /// <summary>
        /// The name of the current category being added
        /// </summary>
        public string PendingAddNewCategoryText { get; set; }
        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand OpenSettingsPageCommand { get; }
        public ICommand OpenNoteListPageCommand { get; }

        private ObservableCollection<CategoryViewModel> Items => _categoryListService.Items;

        private CategoryViewModel ActiveCategory
        {
            get => _categoryListService.ActiveCategory;
            set => _categoryListService.ActiveCategory = value;
        }

        public CategoryPageViewModel()
        {
        }

        public CategoryPageViewModel(
            AppViewModel applicationViewModel,
            IAppContext context,
            OverlayPageService overlayPageService,
            CategoryListService categoryListService,
            MessageService messageService)
        {
            _application = applicationViewModel;
            _context = context;
            _overlayPageService = overlayPageService;
            _categoryListService = categoryListService;
            _messageService = messageService;

            AddCategoryCommand = new RelayCommand(AddCategory);
            DeleteCategoryCommand = new RelayParameterizedCommand(TrashCategory);
            ChangeCategoryCommand = new RelayParameterizedCommand(ChangeCategory);
            OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);

            // Subscribe to the collection changed event for synchronizing with database
            _categoryListService.Items.CollectionChanged += ItemsOnCollectionChanged;

            // Load the application settings to update the ActiveCategory
            _application.LoadAppSettingsOnce();
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

        private void AddCategory()
        {
            // Remove trailing and leading whitespaces
            PendingAddNewCategoryText = PendingAddNewCategoryText.Trim();

            // If the text is empty or only whitespace, refuse
            if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
            {
                return;
            }

            // Create the new category instance
            CategoryViewModel categoryToAdd = new CategoryViewModel
            {
                Name = PendingAddNewCategoryText
            };

            // Untrash category if exists
            var existingCategory = _context.Categories
                .GetAll()
                .FirstOrDefault(x => x.Name.Equals(
                    PendingAddNewCategoryText,
                    StringComparison.InvariantCultureIgnoreCase)).Map();

            if (existingCategory != null && existingCategory.Trashed)
            {
                UntrashCategory(existingCategory);
            }
            else
            {
                AddNewCategory();
            }

            // Reset the input TextBox text
            PendingAddNewCategoryText = string.Empty;
        }

        private void AddNewCategory()
        {
            var activeItems = _context.Categories
                .Where(x => !x.Trashed)
                .OrderByDescending(x => x.ListOrder);

            var lastListOrder = activeItems.Any()
                ? activeItems.First().Map().ListOrder
                : CommonConstants.DefaultListOrder;

            CategoryViewModel categoryToAdd = new CategoryViewModel
            {
                Name = PendingAddNewCategoryText,
                ListOrder = lastListOrder + CommonConstants.ListOrderInterval
            };

            Items.Add(categoryToAdd);
            _context.Categories.Add(categoryToAdd.Map());
        }

        private void UntrashCategory(CategoryViewModel category)
        {
            // First, persist trashed property
            category.Trashed = false;
            _context.Categories.UpdateFirst(category.Map());

            Items.Add(category);
            ReorderingHelperTemp.ReorderCategory(_context, category, Items.Count - 1);

            // Second, persist list order
            _context.Categories.UpdateFirst(category.Map());
        }

        private void TrashCategory(object obj)
        {
            if (obj is CategoryViewModel category)
            {
                // At least one category is required
                if (_context.Categories.Where(x => !x.Trashed).Count > 1)
                {
                    category.Trashed = true;
                    category.ListOrder = CommonConstants.InvalidListOrder;

                    Items.Remove(category);
                    _context.Categories.UpdateFirst(category.Map());

                    // Only if the current category was the deleted one, select a new category
                    if (category == ActiveCategory)
                    {
                        CategoryViewModel firstItem = Items.FirstOrDefault();
                        SetActiveCategory(firstItem);
                    }
                }
                else
                {
                    _messageService.ShowError("Cannot delete last category.");
                }
            }
        }

        private void ChangeCategory(object obj)
        {
            if (obj is CategoryViewModel category)
            {
                IoC.UndoManager.ClearHistory();
                SetActiveCategory(category);
            }
        }

        /// <summary>
        /// Sets the current category to the specified one.
        /// Ensures that always only one IsSelected property is set to true.
        /// </summary>
        /// <param name="category"></param>
        private void SetActiveCategory(CategoryViewModel category)
        {
            if (!string.IsNullOrEmpty(category?.Name))
            {
                if (ActiveCategory != category)
                {
                    ActiveCategory = category;

                    // Notify clients about the category change
                    Mediator.NotifyClients(ViewModelMessages.CategoryChanged);
                    IoC.NoteListService.ActiveNote = null;
                }

                if (IoC.AppSettings.CommonSettings.CloseSideMenuOnCategoryChange)
                {
                    Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
                }

                // Change to task page if it wasn't active
                if (_application.MainPage != ApplicationPage.Task)
                {
                    _application.MainPage = ApplicationPage.Task;
                }
            }
        }

        /// <summary>
        /// Opens the settings page
        /// </summary>
        private void OpenSettingsPage()
        {
            _application.OpenSettingsPage();

            Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
        }

        /// <summary>
        /// Opens the note page
        /// </summary>
        private void OpenNoteListPage()
        {
            _application.SideMenuPage = ApplicationPage.NoteList;
        }

        protected override void OnDispose()
        {
            _categoryListService.Items.CollectionChanged -= ItemsOnCollectionChanged;
        }
    }
}