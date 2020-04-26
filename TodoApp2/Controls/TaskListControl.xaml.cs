using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskListControl.xaml
    /// </summary>
    public partial class TaskListControl : UserControl
    {
        private ListViewDragDropManager<TaskListItemViewModel> m_DragDropManager;

        public TaskListControl()
        {
            InitializeComponent();
            m_DragDropManager = new ListViewDragDropManager<TaskListItemViewModel>(TaskListListView);
        }
    }
}
