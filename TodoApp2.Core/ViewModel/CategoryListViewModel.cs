using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ICommand AddCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
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
            DeleteCategoryCommand = new RelayParameterizedCommand(DeleteCategory);
            ChangeCategoryCommand = new RelayParameterizedCommand(ChangeCategory);

            List<CategoryListItemViewModel> categories = Database.GetCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);

            CategoryListItemViewModel selectedCategory = Items.FirstOrDefault(i => i.Name == CurrentCategory);
            if (selectedCategory != null)
            {
                selectedCategory.IsSelected = true;
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

            // Persist into database if the category is not existed before
            if (Database.AddCategory(categoryToAdd))
            {
                // Add the category into the ViewModel list 
                // only if it is currently added to the database
                Items.Add(categoryToAdd);
            }

            // Reset the input TextBox text
            PendingAddNewCategoryText = string.Empty;
        }

        private void DeleteCategory(object obj)
        {
            if (obj is CategoryListItemViewModel category)
            {
                // At least one category is required
                if (Database.GetCategories().Count > 1)
                {
                    Database.DeleteCategory(category);

                    Items.Remove(category);

                    CategoryListItemViewModel firstItem = Items.FirstOrDefault();
                    firstItem.IsSelected = true;
                    CurrentCategory = firstItem.Name;
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
                CurrentCategory = category.Name;

                foreach (var categoryItem in Items)
                {
                    categoryItem.IsSelected = categoryItem.Name == category.Name;
                }

                IoC.Application.SideMenuVisible = false;
            }
        }
    }

}
