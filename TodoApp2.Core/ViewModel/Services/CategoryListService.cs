using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// The category list items
        /// </summary>
        public ObservableCollection<CategoryListItemViewModel> Items { get; set; }

        /// <summary>
        /// The currently selected category
        /// </summary>
        public string CurrentCategory
        {
            get => m_ApplicationViewModel.ApplicationSettings.CurrentCategory;
            set => m_ApplicationViewModel.ApplicationSettings.CurrentCategory = value;
        }

        public CategoryListService(ApplicationViewModel applicationViewModel, IDatabase database)
        {
            m_ApplicationViewModel = applicationViewModel;
            m_Database = database;

            m_Database.CategoryChanged += OnDatabaseCategoryChanged;

            Mediator.Register(OnOnlineModeChanged, ViewModelMessages.OnlineModeChanged);

            List<CategoryListItemViewModel> categories = m_Database.GetActiveCategories();
            Items = new ObservableCollection<CategoryListItemViewModel>(categories);
        }

        private void OnDatabaseCategoryChanged(object sender, CategoryChangedEventArgs e)
        {
            // Update the category in the app settings
            CurrentCategory = e.ChangedCategory.Name;

            // Update the category in the items list
            CategoryListItemViewModel modifiedItem = Items.FirstOrDefault(item => item.Id == e.ChangedCategory.Id);
            modifiedItem.Name = e.ChangedCategory.Name;
            m_ApplicationViewModel.SaveApplicationSettings();
        }

        public CategoryListItemViewModel GetCurrentCategory => m_Database.GetCategory(CurrentCategory);

        public CategoryListItemViewModel GetCategory(int id) => m_Database.GetCategory(id);
        public CategoryListItemViewModel GetCategory(string categoryName) => m_Database.GetCategory(categoryName);

        private void OnOnlineModeChanged(object obj)
        {
            Items.Clear();
            List<CategoryListItemViewModel> categories = m_Database.GetActiveCategories();
            Items.AddRange(categories);
            OnPropertyChanged(nameof(CurrentCategory));
        }
    }
}
