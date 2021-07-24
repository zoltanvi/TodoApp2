﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model the task list item on the task page
    /// </summary>
    public class TaskPageViewModel : BaseViewModel
    { 
        private readonly TaskListService m_TaskListService;
        private readonly CategoryListService m_CategoryListService;
        private readonly IDatabase m_Database;

        private string CurrentCategory => m_CategoryListService.CurrentCategory;
        private ObservableCollection<TaskListItemViewModel> Items => m_TaskListService.TaskPageItems;

        /// <summary>
        /// The content / description text for the current task being written
        /// </summary>
        public string PendingAddNewTaskText { get; set; }

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
        /// Marks the task item as done
        /// </summary>
        public ICommand TaskIsDoneModifiedCommand { get; }

        /// <summary>
        /// Moves the task item into another category
        /// </summary>
        public ICommand MoveToCategoryCommand { get; }

        public TaskPageViewModel()
        {
        }

        public TaskPageViewModel(TaskListService taskListService, CategoryListService categoryListService, IDatabase database)
        {
            m_TaskListService = taskListService;
            m_CategoryListService = categoryListService;
            m_Database = database;

            AddTaskItemCommand = new RelayCommand(AddTask);
            DeleteTaskItemCommand = new RelayParameterizedCommand(TrashTask);
            DeleteDoneCommand = new RelayCommand(TrashDone);
            PinTaskItemCommand = new RelayParameterizedCommand(Pin);
            UnpinTaskItemCommand = new RelayParameterizedCommand(Unpin);
            DeleteAllCommand = new RelayCommand(TrashAll);
            ResetColorsCommand = new RelayCommand(ResetColors);
            TaskIsDoneModifiedCommand = new RelayParameterizedCommand(ModifyTaskIsDone);
            MoveToCategoryCommand = new RelayParameterizedCommand(MoveToCategory);

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
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

        private void TrashTask(object obj)
        {
            if (obj is TaskListItemViewModel task)
            {
                // Set Trashed property to true so it won't be listed in the active list
                task.Trashed = true;

                // Indicate that it is an invalid order
                task.ListOrder = long.MinValue;

                // Persist modifications
                m_TaskListService.UpdateTask(task);

                // Remove from the list
                m_TaskListService.RemoveTask(task);
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
                m_TaskListService.RemoveTask(item);
            }
        }

        private void ResetColors()
        {
            foreach (TaskListItemViewModel item in Items)
            {
                item.Color = string.Empty;
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
                    parameters[1] is string categoryToMoveTo)
                {
                    // TODO: use CategoryListService call instead
                    CategoryListItemViewModel taskCategory = m_Database.GetCategory(task.CategoryId);

                    // If the category is the same as the task is in, there is nothing to do
                    if (taskCategory.Name != categoryToMoveTo)
                    {
                        // TODO: use CategoryListService call instead
                        CategoryListItemViewModel newCategory = m_Database.GetCategory(categoryToMoveTo);
                        task.CategoryId = newCategory.Id;

                        // Insert into the first correct position.
                        int newIndex = m_TaskListService.GetCorrectReorderIndex(0, task);
                        m_TaskListService.ReorderTask(task, newIndex);

                        // Delete the item from the currently listed items
                        m_TaskListService.RemoveTask(task);
                    }
                }
            }
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
                var taskList = await m_TaskListService.GetActiveTaskItemsAsync(CurrentCategory);

                // 2. Count all pinned items. The currently pinned item is in this list.
                int pinnedItemCount = taskList.Count(i => i.Pinned);

                // 3. Set task to pinned
                task.Pinned = false;

                // 4. Reorder task below the already pinned tasks and above the not-pinned tasks
                m_TaskListService.ReorderTask(task, pinnedItemCount - 1, true);
            }
        }

        public void AddTask()
        {
            // If the text is empty or only whitespace, refuse
            // If the text only contains format characters, refuse
            string trimmed = PendingAddNewTaskText?.Replace("`", string.Empty);
            if (string.IsNullOrWhiteSpace(PendingAddNewTaskText) || string.IsNullOrWhiteSpace(trimmed))
            {
                return;
            }

            // Create the new task instance
            TaskListItemViewModel taskToAdd = new TaskListItemViewModel
            {
                // TODO: use CategoryListService call instead
                CategoryId = m_Database.GetCategory(CurrentCategory).Id,
                Content = PendingAddNewTaskText,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks
            };

            // Add task to list and persist it
            m_TaskListService.AddNewTask(taskToAdd);

            // Reset the input TextBox text
            PendingAddNewTaskText = string.Empty;
        }

        private void MoveTaskToEnd(TaskListItemViewModel task)
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

        private void MoveTaskToTop(TaskListItemViewModel task)
        {
            // Get the valid index. E.g: A normal item cannot be above the pinned ones.
            int newIndex = m_TaskListService.GetCorrectReorderIndex(0, task);
            m_TaskListService.ReorderTask(task, newIndex, true);
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

        protected override void OnDispose()
        {
            Mediator.Deregister(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }
    }
}