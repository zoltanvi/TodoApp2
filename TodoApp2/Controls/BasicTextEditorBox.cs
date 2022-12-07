using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace TodoApp2
{
    public class BasicTextEditorBox : RichTextBox
    {
        private static readonly string s_EmptyContent = XamlWriter.Save(new FlowDocument());
        private static readonly IEnumerable<Block> s_EmptyBlocks = new FlowDocument().Blocks;

        public static readonly DependencyProperty DocumentContentProperty = 
            DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(BasicTextEditorBox), new PropertyMetadata(OnContentChanged));

        public static readonly DependencyProperty IsEmptyProperty = 
            DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(BasicTextEditorBox), new PropertyMetadata());

        /// <summary>
        /// The Document of the <see cref="BasicTextEditorBox"/> serialized into an xml format.
        /// </summary>
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

        /// <summary>
        /// Indicates whether the content of the RichTextBox is empty / whitespace or not.
        /// </summary>
        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValue(IsEmptyProperty, value);
        }

        public BasicTextEditorBox()
        {
            IsEmpty = true;
            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPaste);
        }

        public void UpdateContent()
        {
            string result = XamlWriter.Save(Document);
            DocumentContent = result;
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BasicTextEditorBox textEditorBox && e.NewValue is string newContent)
            {
                textEditorBox.DocumentContent = newContent;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateContent();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            IsEmpty = IsRichTextBoxEmpty(this);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // Prevent pasting an image because it cannot be persisted.
            if (e.FormatToApply == "Bitmap")
            {
                e.CancelCommand();
            }
        }

        private bool IsRichTextBoxEmpty(RichTextBox rtb)
        {
            if (rtb.Document.Blocks.Count == 0)
            {
                return true;
            }

            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            return string.IsNullOrWhiteSpace(textRange.Text);

            //TextPointer startPointer = rtb.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            //TextPointer endPointer = rtb.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            //return endPointer != null && startPointer?.CompareTo(endPointer) == 0;
        }
    }
}
