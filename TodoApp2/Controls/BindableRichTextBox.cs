using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace TodoApp2
{
    public class BindableRichTextBox : RichTextBox
    {
        private static readonly string s_EmptyContent = XamlWriter.Save(new FlowDocument());
        private static readonly IEnumerable<Block> s_EmptyBlocks = new FlowDocument().Blocks;

        public static readonly DependencyProperty DocumentContentProperty =
            DependencyProperty.Register("DocumentContent", typeof(string), typeof(BindableRichTextBox),
                new PropertyMetadata(OnContentChanged));

        public static readonly DependencyProperty IsEmptyProperty =
            DependencyProperty.Register("IsEmpty", typeof(bool), typeof(BindableRichTextBox),
                new PropertyMetadata());


        public string DocumentContent
        {
            get => (string)GetValue(DocumentContentProperty);
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    SetValue(DocumentContentProperty, s_EmptyContent);
                    Document.Blocks.Clear();
                    Document.Blocks.AddRange(s_EmptyBlocks);
                }
                else
                {
                    var xmlReader = System.Xml.XmlReader.Create(new StringReader(value));

                    if (XamlReader.Load(xmlReader) is FlowDocument flowDocument)
                    {
                        Document.Blocks.Clear();
                        Document.Blocks.AddRange(flowDocument.Blocks.ToList());
                        SetValue(DocumentContentProperty, value);
                    }
                }
            }
        }

        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValue(IsEmptyProperty, value);
        }

        public BindableRichTextBox()
        {
            LostFocus += OnLostFocus;
        }


        public void UpdateContent()
        {
            // Update the IsEmpty property
            TextRange textRange = new TextRange(Document.ContentStart, Document.ContentEnd);
            IsEmpty = string.IsNullOrWhiteSpace(textRange.Text);

            // Update the content
            string result = XamlWriter.Save(Document);
            DocumentContent = result;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateContent();
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BindableRichTextBox bindableRichTextBox && e.NewValue is string newContent)
            {
                bindableRichTextBox.DocumentContent = newContent;
            }
        }
    }
}
