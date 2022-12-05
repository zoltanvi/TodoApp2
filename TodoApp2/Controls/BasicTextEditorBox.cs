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

        public static readonly DependencyProperty IsEmptyOrWhiteSpaceProperty = 
            DependencyProperty.Register(nameof(IsEmptyOrWhiteSpace), typeof(bool), typeof(BasicTextEditorBox), new PropertyMetadata());

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
        /// Indicates whether the content of the RichTextBox is empty (or whitespace) or not.
        /// </summary>
        public bool IsEmptyOrWhiteSpace
        {
            get => (bool)GetValue(IsEmptyOrWhiteSpaceProperty);
            set => SetValue(IsEmptyOrWhiteSpaceProperty, value);
        }

        /// <summary>
        /// Indicates whether the content of the RichTextBox is empty or not.
        /// </summary>
        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValue(IsEmptyProperty, value);
        }

        public BasicTextEditorBox()
        {
            IsEmpty = true;
            IsEmptyOrWhiteSpace = true;
            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPaste);
        }

        public void UpdateContent()
        {
            // Update the IsEmptyOrWhiteSpace property.
            // It is not necessary to be updated in every TextChange event
            TextRange textRange = new TextRange(Document.ContentStart, Document.ContentEnd);
            IsEmptyOrWhiteSpace = string.IsNullOrWhiteSpace(textRange.Text);

            // Update the content
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

            TextPointer startPointer = rtb.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = rtb.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            return endPointer != null && startPointer?.CompareTo(endPointer) == 0;
        }
    }
}
