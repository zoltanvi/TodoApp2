using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// The design-time data for a <see cref="TaskListItemViewModel"/>
    /// </summary>
    public class TaskListDesignModel : TaskListViewModel
    {
        private static TaskListDesignModel m_Instance;
        
        public static TaskListDesignModel Instance => m_Instance ?? (m_Instance = new TaskListDesignModel());

        public TaskListDesignModel()
        {
            Items = new List<TaskListItemViewModel>
            {
                new TaskListItemDesignModel
                {
                    Content = "This is my first task, Yaaay!",
                    IsDone = false,
                    Color = "Transparent",
                },

                new TaskListItemDesignModel
                {
                    Content = "This task has quite long text just to test whether it is correctly working or not. Lorem ipsum dolor sit amet, consectetur adipiscing elit. In fringilla sem maximus nibh aliquam, nec molestie sem tempor. Curabitur pretium lectus in lectus rhoncus, vel dignissim arcu eleifend.",
                    IsDone = false,
                    Color = "9C28B1",
                },

                new TaskListItemDesignModel
                {
                    Content = "Finish up this badboii",
                    IsDone = false,
                    Color = "F44236",
                },

                new TaskListItemDesignModel
                {
                    Content = "Finally a task thats finished :)",
                    IsDone = true,
                    Color = "CDDC39",
                },

                new TaskListItemDesignModel
                {
                    Content = "A task that would possibly partially shown",
                    IsDone = true,
                    Color = "4CB050",
                },

                new TaskListItemDesignModel
                {
                    Content = "A task that would possibly partially shown",
                    IsDone = true,
                    Color = "00BCD5",
                },

                new TaskListItemDesignModel
                {
                    Content = "A task that would possibly partially shown",
                    IsDone = true,
                    Color = "FFFFFF",
                },
            };
        }
    }
}
