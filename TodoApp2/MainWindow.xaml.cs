using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
        }

        //private void OnNavigatorBackgroundClicked(object sender, MouseButtonEventArgs e)
        //{
        //    //NavigatorCommand.Execute(null);
        //    IoC.Application.SideMenuVisible ^= true;
        //}
    }
}
