using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class CategoryPageViewModel : BaseViewModel
    {
        private readonly IDatabase _database;
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
            IDatabase database,
            OverlayPageService overlayPageService,
            CategoryListService categoryListService,
            MessageService messageService)
        {
            _application = applicationViewModel;
            _database = database;
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

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
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
                            _database.ReorderCategory(newItem, e.NewStartingIndex);
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

            // Untrash category if it existed before
            CategoryViewModel existingCategory = _database.GetCategory(PendingAddNewCategoryText);

            if (existingCategory != null && existingCategory.Trashed)
            {
                _database.UntrashCategory(existingCategory);
                Items.Add(existingCategory);
            }
            // Persist into database if the category is not existed before
            // Database.AddCategory call can't be optimized because the database gives the ID to the category
            else if (_database.AddCategoryIfNotExists(categoryToAdd))
            {
                // Add the category into the ViewModel list
                // only if it is currently added to the database
                Items.Add(categoryToAdd);
            }

            // Reset the input TextBox text
            PendingAddNewCategoryText = string.Empty;
        }

        private void TrashCategory(object obj)
        {
            if (obj is CategoryViewModel category)
            {
                // At least one category is required
                if (_database.GetValidCategories().Count > 1)
                {
                    _database.TrashCategory(category);

                    Items.Remove(category);

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

        /// <summary>
        /// Forces the UI to repaint the list items when the theme changes
        /// </summary>
        /// <param name="obj"></param>
        private void OnThemeChanged(object obj)
        {
            //Save the current items
            List<CategoryViewModel> itemsBackup = new List<CategoryViewModel>(Items);

            //Clear the items and add back the cleared items to refresh the list(repaint)
            Items.Clear();
            Items.AddRange(itemsBackup);
        }

        protected override void OnDispose()
        {
            _categoryListService.Items.CollectionChanged -= ItemsOnCollectionChanged;

            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}