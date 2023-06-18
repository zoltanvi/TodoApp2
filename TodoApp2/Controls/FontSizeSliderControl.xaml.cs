using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for FontSizeSliderControl.xaml
    /// </summary>
    public partial class FontSizeSliderControl : UserControl
    {
        public static readonly DependencyProperty PreviewFontSizeProperty = DependencyProperty.Register(nameof(PreviewFontSize), typeof(double), typeof(FontSizeSliderControl), new PropertyMetadata());
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(FontSizeSliderControl), new PropertyMetadata());
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(FontSizeSliderControl), new PropertyMetadata());
        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(FontSizeSliderControl), new PropertyMetadata());
        public static readonly DependencyProperty StepsProperty = DependencyProperty.Register(nameof(Steps), typeof(double), typeof(FontSizeSliderControl), new PropertyMetadata());

        public double PreviewFontSize
        {
            get { return (double)GetValue(PreviewFontSizeProperty); }
            set { SetValue(PreviewFontSizeProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }
        
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public double Steps
        {
            get { return (double)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        public FontSizeSliderControl()
        {
            InitializeComponent();
        }
    }
}
