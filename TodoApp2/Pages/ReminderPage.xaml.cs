using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for ReminderPage.xaml
    /// </summary>
    public partial class ReminderPage : BasePage<ReminderPageViewModel>
    {
        public ReminderPage()
        {
            InitializeComponent();
        }

        public ReminderPage(ReminderPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Get the text box
            if (sender is TextBox textBox)
            {
                if (e.Key == Key.Enter)
                {
                }
            }
        }
    }
}