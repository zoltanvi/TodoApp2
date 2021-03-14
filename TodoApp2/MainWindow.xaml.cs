using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowViewModel m_WindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            m_WindowViewModel = new WindowViewModel(this);
            DataContext = m_WindowViewModel;
        }
    }
}