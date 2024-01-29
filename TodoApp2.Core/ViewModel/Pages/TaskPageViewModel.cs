using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TodoApp2.Common;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model the task list item on the task page
    /// </summary>
    public class TaskPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _applicationViewModel;
        private readonly TaskListService _taskListService;
        private readonly CategoryListService _categoryListService;

        private CategoryViewModel ActiveCategory => _categoryListService.ActiveCategory;

        private ObservableCollection<TaskViewModel> Items => _taskListService.TaskPageItems;

        public RichTextEditorViewModel TextEditorViewModel { get; }

        public Action<int> ScrollIntoViewAction { get; set; }

        /// <summary>
        /// The content for the textbox to rename the current category
        /// </summary>
        public string RenameCategoryContent { get; set; }

        /// <summary>
        /// The content / description text for the current task being written
        /// </summary>
        public string AddTaskTextBoxContent
        {
            get => TextEditorViewModel.DocumentContent;
            set => TextEditorViewModel.DocumentContent = value;
        }

        /// <summary>
        /// True if the category name is in edit mode
        /// </summary>
        public bool IsCategoryInEditMode { get; set; }

        /// <summary>
        /// True if the category name is in display mode
        /// </summary>
        public bool IsCategoryInDisplayMode => !IsCategoryInEditMode;

        public bool IsBottomPanelOpen { get; set; } = true;

        public ICommand AddTaskItemCommand { get; }
        public ICommand DeleteTaskItemCommand { get; }
        public ICommand DeleteAllCommand { get; }
        public ICommand DeleteCompletedCommand { get; }
        public ICommand DeleteIncompleteCommand { get; }
        public ICommand PinTaskItemCommand { get; }
        public ICommand UnpinTaskItemCommand { get; }
        public ICommand ResetAllStatesCommand { get; }
        public ICommand ResetColorsCommand { get; }
        public ICommand ResetBorderColorsCommand { get; }
        public ICommand ResetBackgroundColorsCommand { get; }
        public ICommand ResetAllColorsCommand { get; }
        public ICommand TaskIsDoneModifiedCommand { get; }
        public ICommand ToggleTaskIsDoneCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand FinishCategoryEditCommand { get; }
        public ICommand TextBoxFocusedCommand { get; }
        public ICommand SortByStateCommand { get; }
        public ICommand SortByCreationDateCommand { get; }
        public ICommand SortByCreationDateDescCommand { get; }
        public ICommand SortByModificationDateCommand { get; }
        public ICommand SortByModificationDateDescCommand { get; }
        public ICommand MoveToTopCommand { get; }
        public ICommand MoveToBottomCommand { get; }
        public ICommand MoveToCategoryCommand { get; }
        public ICommand MoveAllToCategoryCommand { get; }
        public ICommand MoveAllCompletedToCategoryCommand { get; }
        public ICommand MoveAllIncompleteToCategoryCommand { get; }
        public ICommand SplitLinesCommand { get; }
        public ICommand OpenPageTitleSettingsCommand { get; }

        public ICommand ToggleBottomPanelOpenState { get; }

        public TaskPageViewModel()
        {
        }

        public TaskPageViewModel(AppViewModel applicationViewModel, TaskListService taskListService, CategoryListService categoryListService)
        {
            _applicationViewModel = applicationViewModel;
            _taskListService = taskListService;
            _categoryListService = categoryListService;

            TextEditorViewModel = new RichTextEditorViewModel(false, false, true, true);
            TextEditorViewModel.WatermarkText = "Add new task";
            TextEditorViewModel.EnterAction = CreateTaskItem;
            TextEditorViewModel.OnQuickEditRequestedAction = OnQuickEditRequested;

            AddTaskItemCommand = new UndoableCommand(DoAddTask, RedoAddTask, UndoAddTask);
            DeleteTaskItemCommand = new UndoableCommand(DoTrashTask, RedoTrashTask, UndoTrashTask);

            DeleteCompletedCommand = new RelayCommand(TrashCompleted);
            DeleteIncompleteCommand = new RelayCommand(TrashIncomplete);
            PinTaskItemCommand = new RelayParameterizedCommand<TaskViewModel>(Pin);
            UnpinTaskItemCommand = new RelayParameterizedCommand<TaskViewModel>(Unpin);
            DeleteAllCommand = new RelayCommand(TrashAll);
            ResetAllStatesCommand = new RelayCommand(ResetStates);
            ResetColorsCommand = new RelayCommand(ResetColors);
            ResetBorderColorsCommand = new RelayCommand(ResetBorderColors);
            ResetBackgroundColorsCommand = new RelayCommand(ResetBackgroundColors);
            ResetAllColorsCommand = new RelayCommand(ResetAllColors);

            TaskIsDoneModifiedCommand = new RelayParameterizedCommand<TaskViewModel>(ModifyTaskIsDone);
            ToggleTaskIsDoneCommand = new RelayParameterizedCommand<TaskViewModel>(ToggleTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand<List<object>>(MoveToCategory);
            MoveAllToCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(MoveAllToCategory);
            MoveAllCompletedToCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(MoveAllCompletedToCategory);
            MoveAllIncompleteToCategoryCommand = new RelayParameterizedCommand<CategoryViewModel>(MoveAllIncompleteToCategory);

            SplitLinesCommand = new RelayParameterizedCommand<TaskViewModel>(SplitLines);

            EditCategoryCommand = new RelayCommand(EditCategory);
            FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);
            TextBoxFocusedCommand = new RelayCommand(OnTextBoxFocused);
            
            SortByStateCommand = new RelayCommand(_taskListService.SortByState);
            
            SortByCreationDateCommand = new RelayCommand(_taskListService.SortByCreationDate);
            SortByCreationDateDescCommand = new RelayCommand(_taskListService.SortByCreationDateDesc);

            SortByModificationDateCommand = new RelayCommand(_taskListService.SortByModificationDate);
            SortByModificationDateDescCommand = new RelayCommand(_taskListService.SortByModificationDateDesc);

            MoveToTopCommand = new RelayParameterizedCommand<TaskViewModel>(MoveTaskToTop);
            MoveToBottomCommand = new RelayParameterizedCommand<TaskViewModel>(MoveTaskToBottom);

            OpenPageTitleSettingsCommand = new RelayCommand(OpenPageTitleSettings);

            ToggleBottomPanelOpenState = new RelayCommand(() => IsBottomPanelOpen = !IsBottomPanelOpen);

            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
        }

        private void SplitLines(TaskViewModel model)
        {
            var splitContents = IoC.TaskContentSplitterService.SplitTaskContent(model.Content);

            for (int i = splitContents.Count - 1; i >= 0; i--)
            {
                TaskViewModel task = _taskListService.AddNewTask(splitContents[i]);
            }
        }

        private void OpenPageTitleSettings()
        {
            // TODO: Create a service that opens the appropriate page
        }

        private void OnTextBoxFocused()
        {
            IoC.OneEditorOpenService.EditMode(null);
        }

        private void CreateTaskItem() => AddTaskItemCommand.Execute(null);

        private void UndoTrashTask(CommandObject commandObject)
        {
            if (commandObject?.CommandResult is Tuple<int, TaskViewModel> tuple)
            {
                tuple.Item2.Trashed = false;

                _taskListService.UntrashExistingTask(tuple.Item2, tuple.Item1);
            }
        }

        private CommandObject RedoTrashTask(CommandObject commandObject)
        {
            if (commandObject?.CommandResult is Tuple<int, TaskViewModel> tuple)
            {
                commandObject.CommandArgument = tuple.Item2;
            }
            CommandObject result = DoTrashTask(commandObject);
            return result;
        }

        private CommandObject DoTrashTask(CommandObject commandObject)
        {
            CommandObject result = CommandObject.NotHandled;

            if (commandObject?.CommandArgument is TaskViewModel task)
            {
                int oldPosition = Items.IndexOf(task);

                task.Trashed = true;
                task.ListOrder = CommonConstants.InvalidListOrder;

                _taskListService.UpdateTask(task);

                result = new CommandObject(
                    true,
                    new Tuple<int, TaskViewModel>(oldPosition, task),
                    null,
                    "Task deleted.");
            }

            return result;
        }

        private void OnQuickEditRequested()
        {
            var lastAddedTask = Items.FirstOrDefault(x => x.Id == IoC.OneEditorOpenService.LastEditedTaskId);

            if (lastAddedTask != null)
            {
                int index = Items.IndexOf(lastAddedTask);

                lastAddedTask.EditItemCommand.Execute(null);
                ScrollIntoViewAction?.Invoke(index);
            }
        }

        private CommandObject DoAddTask(CommandObject arg)
        {
            if (TextEditorViewModel.IsContentEmpty)
            {
                return CommandObject.NotHandled;
            }

            // Add task to list and persist it
            TaskViewModel task = _taskListService.AddNewTask(AddTaskTextBoxContent);
            
            // save its ID for quick edit
            IoC.OneEditorOpenService.LastEditedTaskId = task.Id;

            // Reset the input TextBox text
            AddTaskTextBoxContent = string.Empty;

            return new CommandObject(true, task);
        }

        private void UndoAddTask(CommandObject commandObject)
        {
            if (commandObject != null && commandObject.CommandResult is TaskViewModel task)
            {
                task.Trashed = true;
                task.ListOrder = CommonConstants.InvalidListOrder;

                _taskListService.UpdateTask(task);
            }
        }

        private CommandObject RedoAddTask(CommandObject commandObject)
        {
            var result = CommandObject.NotHandled;
            if (commandObject.CommandResult is TaskViewModel task)
            {
                List<TaskViewModel> taskList = _taskListService.GetActiveTaskItems(ActiveCategory);

                int pinnedItemCount = taskList.Count(i => i.Pinned);
                int position = task.Pinned ? 0 : pinnedItemCount;

                _taskListService.UntrashExistingTask(task, position);

                result = new CommandObject(true, task);
            }

            return result;
        }

        private void ModifyTaskIsDone(TaskViewModel task)
        {
            if (task.IsDone)
            {
                // A done item cannot be pinned.
                task.Pinned = false;

                if (IoC.AppSettings.TaskPageSettings.ForceTaskOrderByState)
                {
                    MoveTaskToBottom(task);
                }

                IoC.MediaPlayerService.PlayClick();
            }
            else
            {
                if (IoC.AppSettings.TaskPageSettings.ForceTaskOrderByState)
                {
                    MoveTaskToTop(task);
                }

                IoC.MediaPlayerService.PlayClickReverse();
            }

            _taskListService.UpdateTask(task);
        }

        private void ToggleTaskIsDone(TaskViewModel task)
        {
            task.IsDone = !task.IsDone;
            ModifyTaskIsDone(task);
        }

        private void TrashAll() => TrashTasks(Items);

        private void TrashCompleted() => TrashTasks(Items.Where(i => i.IsDone));

        private void TrashIncomplete() => TrashTasks(Items.Where(i => !i.IsDone));

        private void TrashTasks(IEnumerable<TaskViewModel> taskList)
        {
            ForEachTask(x =>
            {
                x.Trashed = true;
                x.ListOrder = CommonConstants.InvalidListOrder;
            },
            taskList);
        }

        private void ResetStates()
        {
            ForEachTask(x =>
            {
                x.Pinned = false;
                x.IsDone = false;
            });
        }

        private void ResetColors() =>
            ForEachTask(x => x.Color = CoreConstants.ColorName.Transparent);

        private void ResetBorderColors() =>
            ForEachTask(x => x.BorderColor = CoreConstants.ColorName.Transparent);

        private void ResetBackgroundColors() =>
            ForEachTask(x => x.BackgroundColor = CoreConstants.ColorName.Transparent);

        private void ForEachTask(Action<TaskViewModel> action) => ForEachTask(action, Items);

        private void ForEachTask(Action<TaskViewModel> action, IEnumerable<TaskViewModel> taskEnumerable)
        {
            foreach (TaskViewModel item in taskEnumerable)
            {
                action(item);
            }

            _taskListService.PersistTaskList();
        }

        private void ResetAllColors()
        {
            ForEachTask(x =>
            {
                x.Color = CoreConstants.ColorName.Transparent;
                x.BackgroundColor = CoreConstants.ColorName.Transparent;
                x.BorderColor = CoreConstants.ColorName.Transparent;
            });
        }

        /// <summary>
        /// Moves a task into another category
        /// </summary>
        private void MoveToCategory(List<object> parameters)
        {
            if (parameters.Count == 2)
            {
                if (parameters[0] is TaskViewModel task &&
                    parameters[1] is CategoryViewModel categoryToMoveTo)
                {
                    // If the category is the same as the task is in, there is nothing to do
                    if (task.CategoryId != categoryToMoveTo.Id)
                    {
                        task.CategoryId = categoryToMoveTo.Id;

                        // Insert into the first correct position.
                        int newIndex = _taskListService.GetCorrectReorderIndex(0, task, categoryToMoveTo);
                        _taskListService.ReorderTask(task, newIndex);
                    }
                }
            }
        }

        private void MoveAllToCategory(CategoryViewModel categoryToMoveTo)
        {
            List<TaskViewModel> pageItems = Items.ToList();

            MoveItemsToCategory(pageItems, categoryToMoveTo);
        }

        private void MoveAllCompletedToCategory(CategoryViewModel categoryToMoveTo)
        {
            List<TaskViewModel> pageItems = Items
                .Where(x => x.IsDone)
                .ToList();

            MoveItemsToCategory(pageItems, categoryToMoveTo);
        }

        private void MoveAllIncompleteToCategory(CategoryViewModel categoryToMoveTo)
        {
            List<TaskViewModel> pageItems = Items
                .Where(x => !x.IsDone)
                .ToList();

            MoveItemsToCategory(pageItems, categoryToMoveTo);
        }

        private void MoveItemsToCategory(List<TaskViewModel> taskList, CategoryViewModel categoryToMoveTo)
        {
            int newIndex = int.MinValue;

            for (int i = taskList.Count - 1; i >= 0; i--)
            {
                taskList[i].CategoryId = categoryToMoveTo.Id;

                // Get correct reorder index for the first item,
                // then increment the newIndex for all following items
                newIndex = newIndex == int.MinValue
                    ? _taskListService.GetCorrectReorderIndex(0, taskList[i], categoryToMoveTo)
                    : newIndex + 1;

                _taskListService.ReorderTask(taskList[i], newIndex);
            }
        }

        /// <summary>
        /// Enters edit mode to change the category name
        /// </summary>
        private void EditCategory()
        {
            IsCategoryInEditMode = true;
            RenameCategoryContent = _categoryListService.ActiveCategoryName;
        }

        /// <summary>
        /// Exits the edit mode and updates the category name
        /// </summary>
        private void FinishCategoryEdit()
        {
            _categoryListService.ActiveCategoryName = RenameCategoryContent;
            IsCategoryInEditMode = false;
        }

        private void Pin(TaskViewModel task)
        {
            task.Pinned = true;
            task.IsDone = false;

            // Reorder task to the top of the list
            _taskListService.ReorderTask(task, 0, true);
        }

        private void Unpin(TaskViewModel task)
        {
            var taskList = _taskListService.GetActiveTaskItems(ActiveCategory);
            int pinnedItemCount = taskList.Count(i => i.Pinned);

            task.Pinned = false;

            // Reorder task below the already pinned tasks and above the not-pinned tasks
            _taskListService.ReorderTask(task, pinnedItemCount - 1, true);
        }

        private void MoveTaskToBottom(TaskViewModel task)
        {
            int newIndex = _taskListService.GetCorrectReorderIndex(Items.Count - 1, task);
            _taskListService.ReorderTask(task, newIndex, true);
        }

        private void MoveTaskToTop(TaskViewModel task)
        {
            int newIndex = _taskListService.GetCorrectReorderIndex(0, task);
            _taskListService.ReorderTask(task, newIndex, true);
        }

        private void OnCategoryChanged(object obj)
        {
            IsCategoryInEditMode = false;
        }
    }
}