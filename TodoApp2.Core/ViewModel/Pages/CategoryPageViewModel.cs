using Modules.Common.DataBinding;
using Modules.Common.DataModels;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using Modules.Settings.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TodoApp2.Common;
using TodoApp2.Core.Extensions;
using TodoApp2.Core.Mappings;
using TodoApp2.Core.Reordering;
using TodoApp2.Persistence;

namespace TodoApp2.Core;

public class CategoryPageViewModel : BaseViewModel
{
    private readonly IAppContext _context;
    private readonly AppViewModel _application;
    private readonly OverlayPageService _overlayPageService;
    private readonly CategoryListService _categoryListService;
    private readonly TaskListService _taskListService;
    private readonly MessageService _messageService;

    /// <summary>
    /// The name of the current category being added
    /// </summary>
    public string PendingAddNewCategoryText { get; set; }
    public ICommand AddCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand OpenSettingsPageCommand { get; }
    public ICommand OpenNoteListPageCommand { get; }
    public ICommand OpenRecycleBinPageCommand { get; }

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
        TaskListService taskListService,
        MessageService messageService)
    {
        _application = applicationViewModel;
        _context = context;
        _overlayPageService = overlayPageService;
        _categoryListService = categoryListService;
        _taskListService = taskListService;
        _messageService = messageService;

        AddCategoryCommand = new RelayCommand(AddCategory);
        DeleteCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(TrashCategory);
        ChangeCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(ChangeCategory);
        OpenSettingsPageCommand = new RelayCommand(OpenSettingsPage);
        OpenNoteListPageCommand = new RelayCommand(OpenNoteListPage);
        OpenRecycleBinPageCommand = new RelayCommand(OpenRecycleBinPage);

        // Load the application settings to update the ActiveCategory
        _application.LoadAppSettingsOnce();
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
            .Where(x => !x.Trashed && x.Id != CommonConstants.RecycleBinCategoryId)
            .OrderByDescendingListOrder();

        var lastListOrder = activeItems.Any()
            ? activeItems.First().Map().ListOrder
            : CommonConstants.DefaultListOrder;

        CategoryViewModel categoryToAdd = new CategoryViewModel
        {
            Name = PendingAddNewCategoryText,
            ListOrder = lastListOrder + CommonConstants.ListOrderInterval
        };

        var newItem = _context.Categories.Add(categoryToAdd.Map());
        Items.Add(newItem.Map());
    }

    private void UntrashCategory(CategoryViewModel category)
    {
        // First, persist trashed property
        category.Trashed = false;
        _context.Categories.UpdateFirst(category.Map());

        var tasksInCategory = _context.Tasks
                .Where(x => x.CategoryId == category.Id)
                .MapList();

        SetTrashedOnTasks(tasksInCategory, false);

        Items.Add(category);
        ReorderingHelperTemp.ReorderCategory(_context, category, Items.Count - 1);

        // Second, persist list order
        _context.Categories.UpdateFirst(category.Map());
    }

    private void TrashCategory(CategoryViewModel category)
    {
        // At least one category is required
        if (_context.Categories.Where(x => !x.Trashed && x.Id != CommonConstants.RecycleBinCategoryId).Count > 1)
        {
            category.Trashed = true;
            category.ListOrder = CommonConstants.InvalidListOrder;

            var tasksInCategory = _context.Tasks
                .Where(x => x.CategoryId == category.Id)
                .MapList();

            SetTrashedOnTasks(tasksInCategory, true);

            Items.Remove(category);
            _context.Categories.UpdateFirst(category.Map());

            Mediator.NotifyClients(ViewModelMessages.CategoryDeleted);

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

    private void SetTrashedOnTasks(IEnumerable<TaskViewModel> taskList, bool trashed)
    {
        ForEachTask(x =>
        {
            x.Trashed = trashed;
            x.TrashedDate = DateTime.Now.Ticks;
        },
        taskList);
    }

    private void ForEachTask(Action<TaskViewModel> action, IEnumerable<TaskViewModel> taskEnumerable)
    {
        foreach (TaskViewModel item in taskEnumerable)
        {
            action(item);
        }

        _taskListService.PersistTaskList(taskEnumerable);
    }

    private void ChangeCategory(CategoryViewModel category)
    {
        IoC.UndoManager.ClearHistory();
        SetActiveCategory(category);
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

            if (AppSettings.Instance.AppWindowSettings.CloseSideMenuOnCategoryChange)
            {
                Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
            }

            if (category.Id == CommonConstants.RecycleBinCategoryId)
            {
                if (_application.MainPage != ApplicationPage.RecycleBin)
                {
                    // Change to recycle bin page
                    _application.MainPage = ApplicationPage.RecycleBin;
                    _application.MainPageVisible = true;
                }
            }
            else if (_application.MainPage != ApplicationPage.Task)
            {
                // Change to task page if it wasn't active
                _application.MainPage = ApplicationPage.Task;
                _application.MainPageVisible = true;
            }
        }
    }

    /// <summary>
    /// Opens the settings page
    /// </summary>
    private void OpenSettingsPage()
    {
        Mediator.NotifyClients(ViewModelMessages.OpenSettingsPage);

        Mediator.NotifyClients(ViewModelMessages.SideMenuCloseRequested);
    }

    /// <summary>
    /// Opens the note page
    /// </summary>
    private void OpenNoteListPage()
    {
        _application.SideMenuPage = ApplicationPage.NoteList;
    }

    private void OpenRecycleBinPage()
    {
        var recycleBinCategory = _context.Categories
            .First(x => x.Id == CommonConstants.RecycleBinCategoryId)
            .Map();

        SetActiveCategory(recycleBinCategory);
    }
}