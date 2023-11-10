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

        /// <summary>
        /// Adds a new task item
        /// </summary>
        public ICommand AddTaskItemCommand { get; }

        /// <summary>
        /// Deletes the task item
        /// </summary>
        public ICommand DeleteTaskItemCommand { get; }

        /// <summary>
        /// Deletes every task item from the current category
        /// </summary>
        public ICommand DeleteAllCommand { get; }

        /// <summary>
        /// Deletes every DONE task item from the current category
        /// </summary>
        public ICommand DeleteDoneCommand { get; }

        /// <summary>
        /// Pins the task item
        /// </summary>
        public ICommand PinTaskItemCommand { get; }

        /// <summary>
        /// Unpins the task item
        /// </summary>
        public ICommand UnpinTaskItemCommand { get; }

        /// <summary>
        /// Resets each task items color in the current category
        /// </summary>
        public ICommand ResetColorsCommand { get; }

        /// <summary>
        /// Resets each task items border color in the current category
        /// </summary>
        public ICommand ResetBorderColorsCommand { get; }

        /// <summary>
        /// Resets each task items background color in the current category
        /// </summary>
        public ICommand ResetBackgroundColorsCommand { get; }
        public ICommand ResetAllColorsCommand { get; }

        /// <summary>
        /// Saves the task item. The IsDone is modified by the checkbox itself.
        /// </summary>
        public ICommand TaskIsDoneModifiedCommand { get; }

        /// <summary>
        /// Toggles the IsDone property on the task and saves it.
        /// </summary>
        public ICommand ToggleTaskIsDoneCommand { get; }

        /// <summary>
        /// Moves the task item into another category
        /// </summary>
        public ICommand MoveToCategoryCommand { get; }

        /// <summary>
        /// Enters edit mode to change the category name
        /// </summary>
        public ICommand EditCategoryCommand { get; }

        /// <summary>
        /// Finishes edit mode and changes to display mode for the category name
        /// </summary>
        public ICommand FinishCategoryEditCommand { get; }

        /// <summary>
        /// Executes when the Add new task textbox gains focus
        /// </summary>
        public ICommand TextBoxFocusedCommand { get; }

        public ICommand SortByStateCommand { get; }
        public ICommand MoveToTopCommand { get; }
        public ICommand MoveToBottomCommand { get; }

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

            AddTaskItemCommand = new UndoableCommand(DoAddTask, RedoAddTask, UndoAddTask);
            DeleteTaskItemCommand = new UndoableCommand(DoTrashTask, RedoTrashTask, UndoTrashTask);

            DeleteDoneCommand = new RelayCommand(TrashDone);
            PinTaskItemCommand = new RelayParameterizedCommand(Pin);
            UnpinTaskItemCommand = new RelayParameterizedCommand(Unpin);
            DeleteAllCommand = new RelayCommand(TrashAll);
            ResetColorsCommand = new RelayCommand(ResetColors);
            ResetBorderColorsCommand = new RelayCommand(ResetBorderColors);
            ResetBackgroundColorsCommand = new RelayCommand(ResetBackgroundColors);
            ResetAllColorsCommand = new RelayCommand(ResetAllColors);

            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);
            ToggleTaskIsDoneCommand = new RelayParameterizedCommand(ToggleTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand(MoveToCategory);

            EditCategoryCommand = new RelayCommand(EditCategory);
            FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);

            TextBoxFocusedCommand = new RelayCommand(OnTextBoxFocused);
            SortByStateCommand = new RelayCommand(_taskListService.SortTaskPageItems);
            MoveToTopCommand = new RelayParameterizedCommand(MoveToTop);
            MoveToBottomCommand = new RelayParameterizedCommand(MoveToBottom);

            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
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
                int oldPosition = _taskListService.TaskPageItems.IndexOf(task);

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

        private CommandObject DoAddTask(CommandObject arg)
        {
            if (TextEditorViewModel.IsContentEmpty)
            {
                return CommandObject.NotHandled;
            }

            // Add task to list and persist it
            TaskViewModel task = _taskListService.AddNewTask(AddTaskTextBoxContent);

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

        private void ModifyTaskIsDone(object obj)
        {
            if (obj is TaskViewModel task)
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
        }

        private void ToggleTaskIsDone(object obj)
        {
            if (obj is TaskViewModel task)
            {
                task.IsDone = !task.IsDone;
                ModifyTaskIsDone(task);
            }
        }

        private void TrashAll() => TrashTasks(Items);

        private void TrashDone() => TrashTasks(Items.Where(i => i.IsDone));

        private void TrashTasks(IEnumerable<TaskViewModel> taskList)
        {
            ForEachTask(x =>
            {
                x.Trashed = true;
                x.ListOrder = CommonConstants.InvalidListOrder;
            },
            taskList);
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
        private void MoveToCategory(object obj)
        {
            if (obj is List<object> parameters && parameters.Count == 2)
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

        private void Pin(object obj)
        {
            if (obj is TaskViewModel task)
            {
                task.Pinned = true;
                task.IsDone = false;

                // Reorder task to the top of the list
                _taskListService.ReorderTask(task, 0, true);
            }
        }

        private void Unpin(object obj)
        {
            if (obj is TaskViewModel task)
            {
                var taskList = _taskListService.GetActiveTaskItems(ActiveCategory);
                int pinnedItemCount = taskList.Count(i => i.Pinned);

                task.Pinned = false;

                // Reorder task below the already pinned tasks and above the not-pinned tasks
                _taskListService.ReorderTask(task, pinnedItemCount - 1, true);
            }
        }

        private void MoveToBottom(object obj) => MoveTaskToBottom(obj as TaskViewModel);

        private void MoveTaskToBottom(TaskViewModel task)
        {
            if (task == null) return;

            int newIndex = _taskListService.GetCorrectReorderIndex(Items.Count - 1, task);
            _taskListService.ReorderTask(task, newIndex, true);
        }
        private void MoveToTop(object obj) => MoveTaskToTop(obj as TaskViewModel);

        private void MoveTaskToTop(TaskViewModel task)
        {
            if (task == null) return;

            int newIndex = _taskListService.GetCorrectReorderIndex(0, task);
            _taskListService.ReorderTask(task, newIndex, true);
        }

        private void OnCategoryChanged(object obj)
        {
            IsCategoryInEditMode = false;
        }
    }
}