using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    public class TagControl : Label
    {
        public static readonly DependencyProperty TagTitleProperty = DependencyProperty.Register(nameof(TagTitle), typeof(string), typeof(TagControl), new PropertyMetadata());
        public static readonly DependencyProperty TagBodyProperty = DependencyProperty.Register(nameof(TagBody), typeof(string), typeof(TagControl), new PropertyMetadata());
        public static readonly DependencyProperty TagStyleProperty = DependencyProperty.Register(nameof(TagStyle), typeof(string), typeof(TagControl), new PropertyMetadata());

        public string TagTitle
        {
            get { return (string)GetValue(TagTitleProperty); }
            set { SetValue(TagTitleProperty, value); }
        }

        public string TagBody
        {
            get { return (string)GetValue(TagBodyProperty); }
            set { SetValue(TagBodyProperty, value); }
        }

        // E.g: Blue, green, info, warning, etc.
        public string TagStyle
        {
            get { return (string)GetValue(TagStyleProperty); }
            set { SetValue(TagStyleProperty, value); }
        }
    }
}
