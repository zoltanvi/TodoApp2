using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Models;
using Modules.Categories.Views.Controls;
using Modules.Categories.Views.Mappings;
using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Services.Navigation;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace Modules.Categories.Views.Pages;

[AddINotifyPropertyChangedInterface]
public class CategoryPageViewModel : BaseViewModel
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMainPageNavigationService _mainPageNavigationService;
    private readonly ISideMenuPageNavigationService _sideMenuPageNavigationService;
    private CategoryViewModel _activeCategory;
    private int _lastRemovedId = int.MinValue;

    public CategoryPageViewModel(
        ICategoriesRepository categoriesRepository,
        IMainPageNavigationService mainPageNavigationService,
        ISideMenuPageNavigationService sideMenuPageNavigationService)
    {
        ArgumentNullException.ThrowIfNull(categoriesRepository);
        ArgumentNullException.ThrowIfNull(mainPageNavigationService);
        ArgumentNullException.ThrowIfNull(sideMenuPageNavigationService);

        _categoriesRepository = categoriesRepository;
        _mainPageNavigationService = mainPageNavigationService;
        _sideMenuPageNavigationService = sideMenuPageNavigationService;

        AddCategoryCommand = new RelayCommand(AddCategory);
        DeleteCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(DeleteCategory);
        ChangeCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(ChangeCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        // Load the application settings to update the ActiveCategory
        MediatorOBSOLETE.NotifyClients(ViewModelMessages.LoadAppSettings);

        var activeCategories = categoriesRepository.GetActiveCategories();

        var activeCategory = activeCategories
            .First(x => x.Id == AppSettings.Instance.SessionSettings.ActiveCategoryId) ?? activeCategories.First();

        _activeCategory = activeCategory.MapToViewModel();

        Items = new ObservableCollection<CategoryViewModel>(activeCategories.MapToViewModelList());
        Items.CollectionChanged += ItemsOnCollectionChanged;
    }


    public int RecycleBinCategoryId => Constants.RecycleBinCategoryId;
    public string? PendingAddNewCategoryText { get; set; }
    public ICommand AddCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenNoteListPageCommand { get; }
    public ICommand OpenRecycleBinPageCommand { get; }
    public ObservableCollection<CategoryViewModel> Items { get; set; }
    public IEnumerable<CategoryViewModel> InactiveCategories => Items.Where(c => c.Id != ActiveCategory?.Id);

    public string? ActiveCategoryName
    {
        get => ActiveCategory?.Name;

        set
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                ActiveCategory.Name != value)
            {
                ActiveCategory.Name = value;
                UpdateCategory(ActiveCategory);
            }
        }
    }

    public CategoryViewModel ActiveCategory
    {
        get => _activeCategory;
        set
        {
            _activeCategory = value;
            AppSettings.Instance.SessionSettings.ActiveCategoryId = value?.Id ?? -1;
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.SaveAppSettings);
            OnPropertyChanged(nameof(ActiveCategory));
        }
    }

    public void UpdateCategory(CategoryViewModel category)
    {
        if (category.IsDeleted)
        {
            Items.Remove(category);
            _categoriesRepository.DeleteCategory(category.Map());
        }

        var updatedCategory = _categoriesRepository.UpdateCategory(category.Map());

        // Update category in collection
        var pageItem = Items.FirstOrDefault(x => x.Id == category.Id);
        if (pageItem != null)
        {
            var index = Items.IndexOf(pageItem);

            Items.RemoveAt(index);
            Items.Insert(index, updatedCategory.MapToViewModel());
        }
    }

    protected override void OnDispose()
    {
        Items.CollectionChanged -= ItemsOnCollectionChanged;
    }

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].ListOrder = i;
        }

        _categoriesRepository.UpdateCategoryListOrders(Items.MapList());

        // Trigger update to refresh move to category items context menu
        OnPropertyChanged(nameof(InactiveCategories));
    }

    public void RestoreCategoryIfNeeded(int categoryId)
    {
        var existingCategory = _categoriesRepository.GetCategoryById(categoryId);

        // Untrash category if exists
        if (existingCategory?.IsDeleted ?? false)
        {
            RestoreCategory(existingCategory);
        }
    }

    private void RestoreCategory(Category category)
    {
        _categoriesRepository.RestoreCategory(category, Items.Count);

        Items.Add(category.MapToViewModel());
    }

    private void AddCategory()
    {
        // Remove trailing and leading whitespaces
        PendingAddNewCategoryText = PendingAddNewCategoryText?.Trim();

        // If the text is empty or only whitespace, refuse
        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            return;
        }

        // Untrash category if exists
        var existingCategory = _categoriesRepository.GetCategoryByName(PendingAddNewCategoryText);

        if (existingCategory != null && existingCategory.IsDeleted)
        {
            RestoreCategory(existingCategory);
        }
        else
        {
            AddNewCategory();
        }

        // Reset the input TextBox text
        PendingAddNewCategoryText = string.Empty;
        OnPropertyChanged(nameof(PendingAddNewCategoryText));
    }

    private void AddNewCategory()
    {
        if (string.IsNullOrWhiteSpace(PendingAddNewCategoryText))
        {
            throw new InvalidOperationException("Cannot add category with empty name");
        }

        var activeItems = _categoriesRepository.GetActiveCategories();
        var lastListOrder = activeItems.LastOrDefault()?.ListOrder ?? Constants.DefaultListOrder;

        var addedCategory = _categoriesRepository.AddCategory(
            new Category
            {
                Name = PendingAddNewCategoryText,
                ListOrder = lastListOrder + 1
            });

        Items.Add(addedCategory.MapToViewModel());
    }

    private void UntrashCategory(CategoryViewModel category)
    {
        //// First, persist trashed property
        //category.Trashed = false;
        //_context.Categories.UpdateFirst(category.Map());

        //var tasksInCategory = _context.Tasks
        //        .Where(x => x.CategoryId == category.Id)
        //        .MapList();

        //SetTrashedOnTasks(tasksInCategory, false);

        //Items.Add(category);
        //ReorderingHelperTemp.ReorderCategory(_context, category, Items.Count - 1);

        //// Second, persist list order
        //_context.Categories.UpdateFirst(category.Map());
    }

    private void DeleteCategory(CategoryViewModel category)
    {
        // At least one category is required
        var activeCategories = _categoriesRepository.GetActiveCategories();
        if (activeCategories.Count <= 1)
        {
            //_messageService.ShowError("Cannot delete last category.");
            return;
        }

        //category.IsDeleted = true;
        //category.ListOrder = CommonConstants.InvalidListOrder;

        // TODO: DELETE TASKS THAT HAS THIS CATEGORY ID

        Items.Remove(category);
        _categoriesRepository.DeleteCategory(category.Map());

        MediatorOBSOLETE.NotifyClients(ViewModelMessages.CategoryDeleted);

        // Only if the current category was the deleted one, select a new category
        if (category == ActiveCategory)
        {
            SetActiveCategory(Items.FirstOrDefault());
        }

    }

    private void ChangeCategory(CategoryViewModel category)
    {
        //IoC.UndoManager.ClearHistory();
        SetActiveCategory(category);
    }

    /// <summary>
    /// Sets the current category to the specified one.
    /// Ensures that always only one IsSelected property is set to true.
    /// </summary>
    /// <param name="category"></param>
    private void SetActiveCategory(CategoryViewModel? category)
    {
        if (string.IsNullOrWhiteSpace(category?.Name)) return;

        if (ActiveCategory != category)
        {
            ActiveCategory = category;

            // Notify clients about the category change
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.CategoryChanged);
            //IoC.NoteListService.ActiveNote = null;
        }

        if (AppSettings.Instance.AppWindowSettings.CloseSideMenuOnCategoryChange)
        {
            MediatorOBSOLETE.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
        }

        if (category.Id == Constants.RecycleBinCategoryId)
        {
            _mainPageNavigationService.NavigateTo<IRecycleBinPage>();
        }
        else
        {
            _mainPageNavigationService.NavigateTo<ITaskPage>();
        }
    }

    private void OpenSettingsPage()
    {
        _mainPageNavigationService.NavigateTo<ISettingsPage>();
        MediatorOBSOLETE.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
    }

    private void OpenNoteListPage()
    {
        _sideMenuPageNavigationService.NavigateTo<INoteListPage>();
    }

    private void OpenRecycleBinPage()
    {
        var recycleBinCategory = _categoriesRepository.GetCategoryById(Constants.RecycleBinCategoryId);

        SetActiveCategory(recycleBinCategory?.MapToViewModel());
    }
}