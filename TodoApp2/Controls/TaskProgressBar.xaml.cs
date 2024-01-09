using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskProgressBar.xaml
    /// </summary>
    public partial class TaskProgressBar : UserControl
    {
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(int), typeof(TaskProgressBar), new PropertyMetadata(0));

        public static readonly DependencyProperty MaxProgressProperty =
            DependencyProperty.Register(nameof(MaxProgress), typeof(int), typeof(TaskProgressBar), new PropertyMetadata(1));

        public static readonly DependencyProperty NumbersVisibleProperty =
            DependencyProperty.Register(nameof(NumbersVisible), typeof(bool), typeof(TaskProgressBar), new PropertyMetadata(false));

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public int MaxProgress
        {
            get { return (int)GetValue(MaxProgressProperty); }
            set { SetValue(MaxProgressProperty, value); }
        }

        public bool NumbersVisible
        {
            get { return (bool)GetValue(NumbersVisibleProperty); }
            set { SetValue(NumbersVisibleProperty, value); }
        }

        public TaskProgressBar()
        {
            InitializeComponent();
        }
    }

}
