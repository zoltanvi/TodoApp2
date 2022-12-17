using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp2.Core.Constants;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model the task list item on the task page
    /// </summary>
    public class TaskPageViewModel : BaseViewModel
    {
        private readonly ApplicationViewModel m_ApplicationViewModel;
        private readonly TaskListService m_TaskListService;
        private readonly CategoryListService m_CategoryListService;
        private string m_RenameCategoryContentBefore;

        private CategoryListItemViewModel ActiveCategory => m_CategoryListService.ActiveCategory;

        private ObservableCollection<TaskListItemViewModel> Items => m_TaskListService.TaskPageItems;

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
        /// Marks the task item as done
        /// </summary>
        public ICommand TaskIsDoneModifiedCommand { get; }

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


        public TaskPageViewModel()
        {
        }

        public TaskPageViewModel(ApplicationViewModel applicationViewModel, TaskListService taskListService, CategoryListService categoryListService)
        {
            m_ApplicationViewModel = applicationViewModel;
            m_TaskListService = taskListService;
            m_CategoryListService = categoryListService;

            TextEditorViewModel = new RichTextEditorViewModel(false, false, true);
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

            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand(MoveToCategory);

            EditCategoryCommand = new RelayCommand(EditCategory);
            FinishCategoryEditCommand = new RelayCommand(FinishCategoryEdit);

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);

            Mediator.Register(OnCategoryChanged, ViewModelMessages.CategoryChanged);
        }

        private void CreateTaskItem() => AddTaskItemCommand.Execute(null);

        private void UndoTrashTask(CommandObject commandObject)
        {
            if (commandObject?.CommandResult is Tuple<int, TaskListItemViewModel> tuple)
            {
                tuple.Item2.Trashed = false;

                m_TaskListService.UntrashExistingTask(tuple.Item2, tuple.Item1);
            }
        }

        private CommandObject RedoTrashTask(CommandObject commandObject)
        {
            if (commandObject?.CommandResult is Tuple<int, TaskListItemViewModel> tuple)
            {
                commandObject.CommandArgument = tuple.Item2;
            }
            CommandObject result = DoTrashTask(commandObject);
            return result;
        }

        private CommandObject DoTrashTask(CommandObject commandObject)
        {
            CommandObject result = CommandObject.NotHandled;

            if (commandObject?.CommandArgument is TaskListItemViewModel task)
            {
                int oldPosition = m_TaskListService.TaskPageItems.IndexOf(task);

                // Set Trashed property to true so it won't be listed in the active list
                task.Trashed = true;

                // Indicate that it is an invalid order
                task.ListOrder = long.MinValue;

                // Persist modifications
                m_TaskListService.UpdateTask(task);

                // Remove from the list
                m_TaskListService.RemoveTaskFromMemory(task);

                result = new CommandObject(true, new Tuple<int, TaskListItemViewModel>(oldPosition, task), null,
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
            TaskListItemViewModel task = m_TaskListService.AddNewTask(AddTaskTextBoxContent);

            // Reset the input TextBox text
            AddTaskTextBoxContent = string.Empty;

            return new CommandObject(true, task);
        }

        private void UndoAddTask(CommandObject commandObject)
        {
            if (commandObject != null && commandObject.CommandResult is TaskListItemViewModel task)
            {
                task.Trashed = true;
                task.ListOrder = long.MinValue;
                m_TaskListService.UpdateTask(task);
                m_TaskListService.RemoveTaskFromMemory(task);
            }
        }

        private CommandObject RedoAddTask(CommandObject commandObject)
        {
            var result = CommandObject.NotHandled;
            if (commandObject.CommandResult is TaskListItemViewModel task)
            {
                List<TaskListItemViewModel> taskList =
                    Task.Run(() => m_TaskListService.GetActiveTaskItemsAsync(ActiveCategory)).Result;

                int pinnedItemCount = taskList.Count(i => i.Pinned);
                int position = task.Pinned ? 0 : pinnedItemCount;

                m_TaskListService.UntrashExistingTask(task, position);

                result = new CommandObject(true, task);
            }

            return result;
        }

        private void ModifyTaskIsDone(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // If this task is done, move it after the last not done item
                // If this is not done (undone action), move it to the top of the list
                // Because this generates a NotifyCollectionChangedAction.Move action,
                // all task modifications will be persisted
                if (task.IsDone)
                {
                    // A done item cannot be pinned.
                    task.Pinned = false;
                    MoveTaskToEnd(task);
                }
                else
                {
                    MoveTaskToTop(task);
                }
            }
        }


        /// <inheritdoc cref="DeleteAllCommand"/>
        private void TrashAll()
        {
            TrashTasks(Items);
        }

        /// <inheritdoc cref="DeleteDoneCommand"/>
        private void TrashDone()
        {
            TrashTasks(Items.Where(i => i.IsDone));
        }

        /// <summary>
        /// Trashes every item from the <paramref name="taskList"/>.
        /// Only persists the items 
        /// </summary>
        /// <param name="taskList"></param>
        private void TrashTasks(IEnumerable<TaskListItemViewModel> taskList)
        {
            var items = new List<TaskListItemViewModel>(taskList);

            foreach (TaskListItemViewModel item in items)
            {
                // Set Trashed property to true so it won't be listed in the active list
                item.Trashed = true;

                // Indicate that it is an invalid order
                item.ListOrder = long.MinValue;
            }

            m_TaskListService.PersistTaskList();

            foreach (TaskListItemViewModel item in items)
            {
                m_TaskListService.RemoveTaskFromMemory(item);
            }
        }

        private void ResetColors()
        {
            foreach (TaskListItemViewModel item in Items)
            {
                item.Color = GlobalConstants.ColorName.Transparent;
            }

            m_TaskListService.PersistTaskList();
        }

        private void ResetBorderColors()
        {
            foreach (TaskListItemViewModel item in Items)
            {
                item.BorderColor = GlobalConstants.ColorName.Transparent;
            }

            m_TaskListService.PersistTaskList();
        }

        /// <summary>
        /// Moves a task into another category
        /// </summary>
        /// <param name="obj"></param>
        private void MoveToCategory(object obj)
        {
            if (obj is List<object> parameters && parameters.Count == 2)
            {
                if (parameters[0] is TaskListItemViewModel task &&
                    parameters[1] is CategoryListItemViewModel categoryToMoveTo)
                {
                    // If the category is the same as the task is in, there is nothing to do
                    if (task.CategoryId != categoryToMoveTo.Id)
                    {
                        //CategoryListItemViewModel newCategory = m_CategoryListService.GetCategory(categoryToMoveTo);
                        task.CategoryId = categoryToMoveTo.Id;

                        // Insert into the first correct position.
                        int newIndex = m_TaskListService.GetCorrectReorderIndex(0, task, categoryToMoveTo);
                        m_TaskListService.ReorderTask(task, newIndex);

                        // Delete the item from the currently listed items
                        m_TaskListService.RemoveTaskFromMemory(task);
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
            RenameCategoryContent = m_CategoryListService.ActiveCategoryName;
            m_RenameCategoryContentBefore = RenameCategoryContent;
        }

        /// <summary>
        /// Exits the edit mode and updates the category name
        /// </summary>
        private void FinishCategoryEdit()
        {
            m_CategoryListService.ActiveCategoryName = RenameCategoryContent;
            IsCategoryInEditMode = false;
        }

        private void Pin(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // 1. Set task to pinned
                task.Pinned = true;

                // 2. A pinned task is not done yet.
                task.IsDone = false;

                // 3. Reorder task to the top of the list
                m_TaskListService.ReorderTask(task, 0, true);
            }
        }

        private async void Unpin(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // 1. Get the tasks in the category
                var taskList = await m_TaskListService.GetActiveTaskItemsAsync(ActiveCategory);

                // 2. Count all pinned items. The currently pinned item is in this list.
                int pinnedItemCount = taskList.Count(i => i.Pinned);

                // 3. Set task to not pinned
                task.Pinned = false;

                // 4. Reorder task below the already pinned tasks and above the not-pinned tasks
                m_TaskListService.ReorderTask(task, pinnedItemCount - 1, true);
            }
        }

        private void MoveTaskToEnd(TaskListItemViewModel task)
        {
            if (m_ApplicationViewModel.ApplicationSettings.MoveTaskOnCompletion)
            {
                int newIndex = Items.Count - 1;

                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    newIndex = i;
                    if (Items[i].Equals(task) || Items[i].IsDone == false)
                    {
                        break;
                    }
                }

                m_TaskListService.ReorderTask(task, newIndex, true);
            }
        }

        private void MoveTaskToTop(TaskListItemViewModel task)
        {
            if (m_ApplicationViewModel.ApplicationSettings.MoveTaskOnCompletion)
            {
                // Get the valid index. E.g: A normal item cannot be above the pinned ones.
                int newIndex = m_TaskListService.GetCorrectReorderIndex(0, task);
                m_TaskListService.ReorderTask(task, newIndex, true);
            }
        }

        /// <summary>
        /// Forces the UI to repaint the list items when the theme changes
        /// </summary>
        /// <param name="obj"></param>
        private void OnThemeChanged(object obj)
        {
            // Save the current items
            List<TaskListItemViewModel> itemsBackup = new List<TaskListItemViewModel>(Items);

            // Clear the items and add back the cleared items to refresh the list (repaint)
            Items.Clear();
            Items.AddRange(itemsBackup);
        }

        private void OnCategoryChanged(object obj)
        {
            IsCategoryInEditMode = false;
        }

        protected override void OnDispose()
        {
            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}