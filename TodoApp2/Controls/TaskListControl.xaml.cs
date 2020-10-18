using System.Windows.Controls;
using TodoApp2.Core;
using WPF.JoshSmith.ServiceProviders.UI;

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
            InitializeDragDropManager();
        }
        private void InitializeDragDropManager()
        {
            m_DragDropManager = new ListViewDragDropManager<TaskListItemViewModel>(TaskListListView);
            m_DragDropManager.ShowDragAdorner = false;
        }
    }
}
