using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class CategoryPageViewModel : BaseViewModel
    {
        private readonly IDatabase m_Database;
        private readonly ApplicationViewModel m_Application;
        private readonly OverlayPageService m_OverlayPageService;
        private readonly CategoryListService m_CategoryListService;
        private readonly MessageService m_MessageService;


        private int m_LastRemovedId = int.MinValue;

        /// <summary>
        /// The name of the current category being added
        /// </summary>
        public string PendingAddNewCategoryText { get; set; }

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand OpenSettingsPageCommand { get; }
        public ICommand LogInCommand { get; }
        public ICommand LogOutCommand { get; }

        private ObservableCollection<CategoryListItemViewModel> Items => m_CategoryListService.Items;

        private string CurrentCategory
        {
            get => m_CategoryListService.CurrentCategory;
            set => m_CategoryListService.CurrentCategory = value;
        }

        public CategoryPageViewModel()
        {
        }

        public CategoryPageViewModel(ApplicationViewModel applicationViewModel, IDatabase database,
            OverlayPageService overlayPageService, CategoryListService categoryListService, MessageService messageService)
        {
            m_Application = applicationViewModel;
            m_Database = database;
            m_OverlayPageService = overlayPageService;
            m_CategoryListService = categoryListService;
            m_MessageService = messageService;

            AddCategoryCommand = new RelayCommand(AddCategory);
            DeleteCategoryCommand = new RelayParameterizedCommand(TrashCategory);
            ChangeCategoryCommand = new RelayParameterizedCommand(ChangeCategory);
            OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
            LogInCommand = new RelayCommand(IoC.SessionManager.LogIn);
            LogOutCommand = new RelayCommand(IoC.SessionManager.LogOut);

            // Subscribe to the collection changed event for synchronizing with database
            m_CategoryListService.Items.CollectionChanged += ItemsOnCollectionChanged;

            // Load the application settings to update the CurrentCategory
            m_Application.LoadApplicationSettingsOnce();

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
                        var newItem = (CategoryListItemViewModel)e.NewItems[0];

                        // If the newly added item is the same as the last deleted one,
                        // then this was a drag and drop reorder
                        if (newItem.Id == m_LastRemovedId)
                        {
                            m_Database.ReorderCategory(newItem, e.NewStartingIndex);
                        }

                        m_LastRemovedId = int.MinValue;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems.Count > 0)
                    {
                        var last = (CategoryListItemViewModel)e.OldItems[0];

                        m_LastRemovedId = last.Id;
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
            CategoryListItemViewModel categoryToAdd = new CategoryListItemViewModel
            {
                Name = PendingAddNewCategoryText
            };

            // Untrash category if it existed before
            CategoryListItemViewModel existingCategory = m_Database.GetCategory(PendingAddNewCategoryText);

            if (existingCategory != null && existingCategory.Trashed)
            {
                m_Database.UntrashCategory(existingCategory);
                Items.Add(existingCategory);
            }
            // Persist into database if the category is not existed before
            // Database.AddCategory call can't be optimized because the database gives the ID to the category
            else if (m_Database.AddCategoryIfNotExists(categoryToAdd))
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
            if (obj is CategoryListItemViewModel category)
            {
                // At least one category is required
                if (m_Database.GetActiveCategories().Count > 1)
                {
                    m_Database.TrashCategory(category);

                    Items.Remove(category);

                    // Only if the current category was the deleted one, select a new category
                    if (category.Name == CurrentCategory)
                    {
                        CategoryListItemViewModel firstItem = Items.FirstOrDefault();
                        SetCurrentCategory(firstItem?.Name);
                    }
                }
                else
                {
                    m_MessageService.ShowError("Cannot delete last category.");
                }
            }
        }

        private void ChangeCategory(object obj)
        {
            if (obj is CategoryListItemViewModel category)
            {
                SetCurrentCategory(category.Name);
            }
        }

        /// <summary>
        /// Sets the current category to the specified one.
        /// Ensures that always only one IsSelected property is set to true.
        /// </summary>
        /// <param name="categoryName"></param>
        private void SetCurrentCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                if (CurrentCategory != categoryName)
                {
                    // Set the CurrentCategory property
                    CurrentCategory = categoryName;

                    // Notify clients about the category change
                    Mediator.NotifyClients(ViewModelMessages.CategoryChanged);
                }

                m_OverlayPageService.CloseSideMenu();

                // Change to task page if it wasn't active
                if (m_Application.CurrentPage != ApplicationPage.Task)
                {
                    m_Application.CurrentPage = ApplicationPage.Task;
                }
            }
        }

        /// <summary>
        /// Opens the settings page
        /// </summary>
        private void OpenSettingsPage()
        {
            m_Application.SideMenuPage = ApplicationPage.Settings;
        }

        /// <summary>
        /// Forces the UI to repaint the list items when the theme changes
        /// </summary>
        /// <param name="obj"></param>
        private void OnThemeChanged(object obj)
        {
            //Save the current items
            List<CategoryListItemViewModel> itemsBackup = new List<CategoryListItemViewModel>(Items);

            //Clear the items and add back the cleared items to refresh the list(repaint)
            Items.Clear();
            Items.AddRange(itemsBackup);
        }

        protected override void OnDispose()
        {
            m_CategoryListService.Items.CollectionChanged -= ItemsOnCollectionChanged;

            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}