using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskListItemControl.xaml
    /// </summary>
    public partial class TaskListItemControl : UserControl
    {
        public TaskListItemControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is TaskListItemViewModel viewModel)
            {
                TextBoxPreviewKeyDownHelper.TextBox_PreviewKeyDown(sender, e, viewModel.UpdateContent);
            }
        }
    }
}