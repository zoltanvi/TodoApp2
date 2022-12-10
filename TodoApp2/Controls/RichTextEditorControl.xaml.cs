using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for RichTextEditorControl.xaml
    /// </summary>
    public partial class RichTextEditorControl : UserControl
    {
        public static readonly DependencyProperty TextOpacityProperty = DependencyProperty.Register(nameof(TextOpacity), typeof(double), typeof(RichTextEditorControl), new PropertyMetadata());

        public double TextOpacity
        {
            get { return (double)GetValue(TextOpacityProperty); }
            set { SetValue(TextOpacityProperty, value); }
        }

        public RichTextEditorControl()
        {
            InitializeComponent();
        }
    }
}
