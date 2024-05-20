using System;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridResizer _gridResizer;

        public MainWindow(MainWindowViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            InitializeComponent();
            DataContext = viewModel;
            viewModel.Window = this;
            //DataContext = new MainWindowViewModel(this, IoC.AppViewModel, IoC.Context);
            _gridResizer = new GridResizer(Grid, Resizer, this);
        }

        private void AppWindow_Deactivated(object sender, EventArgs e)
        {
            (DataContext as MainWindowViewModel).Active = false;
        }

        private void AppWindow_Activated(object sender, EventArgs e)
        {
            (DataContext as MainWindowViewModel).Active = true;
        }
    }
}