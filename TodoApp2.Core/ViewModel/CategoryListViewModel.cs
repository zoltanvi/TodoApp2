using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class CategoryListViewModel : BaseViewModel
    {
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

        private ClientDatabase Database => IoC.Get<ClientDatabase>();

        private string CurrentCategory
        {
            get => IoC.Application.CurrentCategory;

            set
            {
                IoC.Application.CurrentCategory = value;
                // Notify all listeners about the category change
                Mediator.Instance.NotifyClients(ViewModelMessages.CategoryChanged, value);
            }
        }

        public CategoryListViewModel()
        {
            AddCategoryCommand = new RelayCommand(AddCategory);
            DeleteCategoryCommand = new RelayParameterizedCommand(TrashCategory);
            ChangeCategoryCommand = new RelayParameterizedCommand(ChangeCategory);

            List<CategoryListItemViewModel> categories = Database.GetActiveCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);

            // Subscribe to the collection changed event for synchronizing with database 
            Items.CollectionChanged += ItemsOnCollectionChanged;

            // TODO: Load back from the view settings table the selected category here
            CategoryListItemViewModel selectedCategory = categories.FirstOrDefault();
            SetCurrentCategory(selectedCategory?.Name);
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                {
                    // Persist the reordered list into database
                    // TODO: optimize the reordering
                    Database.UpdateCategoryListOrders(Items);
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
                Name = PendingAddNewCategoryText,
                ListOrder = Items.Count
            };

            // Untrash category if it existed before
            CategoryListItemViewModel untrashedCategory = Database.UntrashCategoryIfExists(categoryToAdd);

            if (untrashedCategory != null)
            {
                Items.Add(untrashedCategory);
            }
            // Persist into database if the category is not existed before
            // Database.AddCategory call can't be optimized because the database gives the ID to the category
            else if (Database.AddCategory(categoryToAdd))
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

                IoC.Application.SideMenuVisible = false;
            }
        }

        /// <summary>
        /// Sets the current category to the specified one.
        /// Ensures that always only one IsSelected property is set to true.
        /// </summary>
        /// <param name="categoryName"></param>
        private void SetCurrentCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return;
            }

            // Set every IsSelected property to false, except for the current category
            foreach (var categoryItem in Items)
            {
                categoryItem.IsSelected = categoryItem.Name == categoryName;
            }

            // Set the CurrentCategory property
            CurrentCategory = categoryName;
        }
    }

}
