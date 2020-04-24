using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TodoApp2.ViewModel;

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
