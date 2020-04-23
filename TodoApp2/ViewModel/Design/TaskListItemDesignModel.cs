using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2
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
