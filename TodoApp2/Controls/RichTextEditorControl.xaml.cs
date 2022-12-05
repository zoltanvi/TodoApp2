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

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for RichTextEditorControl.xaml
    /// </summary>
    public partial class RichTextEditorControl : UserControl
    {
        public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(nameof(EditMode), typeof(bool), typeof(RichTextEditorControl), new PropertyMetadata());

        public bool EditMode
        {
            get => (bool)GetValue(EditModeProperty);
            set => SetValue(EditModeProperty, value);
        }

        public RichTextEditorControl()
        {
            InitializeComponent();
        }
    }
}
