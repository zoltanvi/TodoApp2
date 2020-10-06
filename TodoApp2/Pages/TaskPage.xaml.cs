using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : BasePage<TaskPageViewModel>
    {
        public TaskPage()
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.Closing += MainWindowOnClosing;
            }
        }

        /// <summary>
        /// When the application get closed somehow (intentionally or unintentionally)
        /// try to save the task items into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            ViewModel.PersistTaskList();
        }

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewTaskTextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Add task on enter, handle modifier keys
            TextBoxPreviewKeyDownHelper.TextBox_PreviewKeyDown(sender, e, ViewModel.AddTask);
        }


        //private void AddNewTaskTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var heightDiff = e.NewSize.Height - e.PreviousSize.Height;
        //    const double marginOffset = 23.939999999999998;
        //    const int maxHeight = 200;

        //    if (e.PreviousSize.Height < e.NewSize.Height && heightDiff < marginOffset && marginOffset + e.NewSize.Height < maxHeight)
        //    {
        //        MyTaskListControl.Margin = ModifyMargin(MyTaskListControl.Margin, 0, 0, 0, marginOffset);
        //        BottomBorderPanel.Margin = ModifyMargin(BottomBorderPanel.Margin, 0, - marginOffset, 0, 0);
        //        BottomBorderPanel.Height += marginOffset;
        //    }
        //    else
        //    {

        //    }
        //    //else if(e.PreviousSize.Height > e.NewSize.Height)
        //}

        //private Thickness ModifyMargin(Thickness original, double left, double top, double right, double bottom)
        //{
        //    return new Thickness(original.Left + left, original.Top + top, original.Right + right, original.Bottom + bottom);
        //}
    }
}
