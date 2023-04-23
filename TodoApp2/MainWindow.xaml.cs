using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppViewModel AppViewModel => IoC.ApplicationViewModel;
        private GridResizer _GridResizer;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this, AppViewModel, IoC.Database);
            _GridResizer = new GridResizer(Grid, Resizer, this);
        }
    }
}