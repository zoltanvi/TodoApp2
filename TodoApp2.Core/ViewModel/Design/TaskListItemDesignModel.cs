namespace TodoApp2.Core
{
    /// <summary>
    /// The design-time data for a <see cref="TaskListItemViewModel"/>
    /// </summary>
    public class TaskListItemDesignModel : TaskListItemViewModel
    {
        private static TaskListItemDesignModel m_Instance;
        
        public static TaskListItemDesignModel Instance => m_Instance ?? (m_Instance = new TaskListItemDesignModel());

        public TaskListItemDesignModel()
        {

        }
    }
}
