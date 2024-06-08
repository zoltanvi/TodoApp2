using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for SideMenuControl.xaml
    /// </summary>
    public partial class SideMenuControl : UserControl
    {
        public static readonly DependencyProperty SideMenuFrameProperty =
            DependencyProperty.Register(nameof(SideMenuFrame), typeof(Frame), typeof(SideMenuControl));

        public Frame SideMenuFrame
        {
            get { return (Frame)GetValue(SideMenuFrameProperty); }
            set { SetValue(SideMenuFrameProperty, value); }
        }

        public SideMenuControl()
        {
            InitializeComponent();
            Loaded += SideMenuControl_Loaded;
        }

        private void SideMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            SideMenuFrame = (Frame)Template.FindName("SideMenuFrameElement", this);
        }
    }
}