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
        private ApplicationViewModel m_ApplicationViewModel;
        public ApplicationViewModel MyApplicationViewModel => m_ApplicationViewModel ?? (m_ApplicationViewModel = new ApplicationViewModel());

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MyApplicationViewModel.SideMenuVisible ^= true;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MyApplicationViewModel.SideMenuVisible ^= true;
        }
    }
}
