using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class CategoryPageViewModel : BaseViewModel
    {
        private int m_LastRemovedId = int.MinValue;

        /// <summary>
        /// The task list items for the list
        /// </summary>
        public ObservableCollection<CategoryListItemViewModel> Items { get; set; }

        /// <summary>
        /// The content / description text for the current task being written
        /// </summary>
        public string PendingAddNewCategoryText { get; set; }

        /// <summary>
        /// The command for when the user presses Enter in the "Add new category" TextBox
        /// </summary>
        public ICommand AddCategoryCommand { get; }

        /// <summary>
        /// The command for when the trash button is pressed in the category item
        /// </summary>
        public ICommand DeleteCategoryCommand { get; }

        /// <summary>
        /// The command for when the category item is clicked
        /// </summary>
        public ICommand ChangeCategoryCommand { get; }

        private ClientDatabase Database => IoC.ClientDatabase;

        private ApplicationViewModel Application => IoC.Application;

        private string CurrentCategory
        {
            get => Application.CurrentCategory;
            set => Application.CurrentCategory = value;
        }

        public CategoryPageViewModel()
        {
            AddCategoryCommand = new RelayCommand(AddCategory);
            DeleteCategoryCommand = new RelayParameterizedCommand(TrashCategory);
            ChangeCategoryCommand = new RelayParameterizedCommand(ChangeCategory);

            List<CategoryListItemViewModel> categories = Database.GetActiveCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);

            // Subscribe to the collection changed event for synchronizing with database 
            Items.CollectionChanged += ItemsOnCollectionChanged;

            // Load the application settings to update the CurrentCategory
            Application.LoadApplicationSettingsOnce();

            // Get the current category from the database
            CategoryListItemViewModel selectedCategory = Database.GetCategory(Application.ApplicationSettings.CurrentCategory);
            SetCurrentCategory(selectedCategory?.Name);
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
                            Database.ReorderCategory(newItem, e.NewStartingIndex);
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
            CategoryListItemViewModel existingCategory = Database.GetCategory(PendingAddNewCategoryText);

            if (existingCategory != null && existingCategory.Trashed)
            {
                Database.UntrashCategory(existingCategory);
                Items.Add(existingCategory);
            }
            // Persist into database if the category is not existed before
            // Database.AddCategory call can't be optimized because the database gives the ID to the category
            else if (Database.AddCategoryIfNotExists(categoryToAdd))
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
                if (Database.GetActiveCategories().Count > 1)
                {
                    Database.TrashCategory(category);

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
                    // TODO: error message
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
                // Set every IsSelected property to false, except for the current category
                foreach (var categoryItem in Items)
                {
                    categoryItem.IsSelected = categoryItem.Name == categoryName;
                }

                // Set the CurrentCategory property
                CurrentCategory = categoryName;

                // Notify clients about the category change
                Mediator.Instance.NotifyClients(ViewModelMessages.CategoryChanged);
            }
        }

    }
}
