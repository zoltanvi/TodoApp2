using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ThemeEditor.xaml
    /// </summary>
    public partial class ThemeEditor : Window
    {
        public ThemeEditor()
        {
            InitializeComponent();
            DataContext = new ThemeEditorViewModel();
        }

        internal ThemeEditor(ThemeManager themeManager)
        {
            InitializeComponent();
            DataContext = new ThemeEditorViewModel(themeManager);
        }
    }
}
