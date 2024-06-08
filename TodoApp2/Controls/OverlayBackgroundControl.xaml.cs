using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for OverlayBackgroundControl.xaml
    /// </summary>
    public partial class OverlayBackgroundControl : UserControl
    {
        public static readonly DependencyProperty OverlayFrameProperty = 
            DependencyProperty.Register(nameof(OverlayFrame), typeof(Frame), typeof(OverlayBackgroundControl));

        public Frame OverlayFrame
        {
            get { return (Frame)GetValue(OverlayFrameProperty); }
            set { SetValue(OverlayFrameProperty, value); }
        }

        public OverlayBackgroundControl()
        {
            InitializeComponent();
            Loaded += SideMenuControl_Loaded;
        }

        private void SideMenuControl_Loaded(object sender, RoutedEventArgs e)
        {
            OverlayFrame = (Frame)Template.FindName("OverlayFrameElement", this);
        }
    }
}