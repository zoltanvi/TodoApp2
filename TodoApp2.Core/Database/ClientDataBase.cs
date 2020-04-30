using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TodoApp2.Core
{
    /// <summary>
    /// Represents the application client database
    /// </summary>
    public class ClientDatabase
    {


        public ClientDatabase()
        {
            // Initialize the database
            DataAccessLayer.InitializeDatabase();
        }

        public List<TaskListItemViewModel> GetActiveTaskItems(string categoryName)
        {
            // Returns the task list from the database ordered by ListOrder column
            List<TaskListItemViewModel> allTasks = DataAccessLayer.GetTasks(categoryName);
            
            // Returns only the items that are not trashed
            return allTasks.Where(task => task.Trashed == false).ToList();
        }

        public void AddTask(TaskListItemViewModel taskListItem)
        {
            // Add task to database, get back the generated ID
            int taskId = DataAccessLayer.AddTask(taskListItem);

            // Set the generated ID to the task in the memory
            taskListItem.Id = taskId;
        }

        internal void UpdateTaskListOrders(ObservableCollection<TaskListItemViewModel> taskListItems)
        {
            // Update the ListOrder property of the TaskItems with their order in the provided list
            for (int i = 0; i < taskListItems.Count; i++)
            {
                taskListItems[i].ListOrder = i;
            }

            // Persist the changes into the database
            DataAccessLayer.UpdateTaskListOrders(taskListItems);
        }

        internal void UpdateTaskList(ObservableCollection<TaskListItemViewModel> items)
        {
            //throw new NotImplementedException();
        }

        internal void TrashTask(TaskListItemViewModel task)
        {
            // Set Trashed property to true so it won't be listed in the active list
            task.Trashed = true;
            
            // Indicate that it is an invalid order
            task.ListOrder = -1;

            // Persist the change into the database
            DataAccessLayer.UpdateTask(task);
        }
    }
}
