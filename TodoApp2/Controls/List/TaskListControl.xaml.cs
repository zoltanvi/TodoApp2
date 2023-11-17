using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskListControl.xaml
    /// </summary>
    public partial class TaskListControl : UserControl
    {
        public TaskListControl()
        {
            InitializeComponent();
        }

        public void ScrollToItem(int index)
        {
            var itemToScrollTo = TaskListListView.Items.GetItemAt(index);
            if (itemToScrollTo != null)
            {
                TaskListListView.ScrollIntoView(itemToScrollTo);
            }
        }
    }
}