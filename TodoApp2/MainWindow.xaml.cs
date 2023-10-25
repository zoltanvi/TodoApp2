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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this, IoC.AppViewModel, IoC.Database);
            _gridResizer = new GridResizer(Grid, Resizer, this);
        }
    }
}