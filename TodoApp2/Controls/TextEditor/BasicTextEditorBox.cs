using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TodoApp2.Helpers;

namespace TodoApp2
{
    public class BasicTextEditorBox : RichTextBox
    {
        internal static readonly DependencyPropertyKey IsEmptyPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsEmpty), typeof(bool), typeof(BasicTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty DocumentContentProperty = DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(BasicTextEditorBox), new PropertyMetadata(OnContentChanged));

        private bool _setContentInProgress = false;

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
                    SetValue(DocumentContentProperty, FlowDocumentHelper.EmptySerializedDocument);
                }
                else if (!_setContentInProgress)
                {
                    _setContentInProgress = true;
                    
                    if (FlowDocumentHelper.DeserializeDocument(value) is FlowDocument flowDocument)
                    {
                        SetValue(DocumentContentProperty, value);
                        Document.Blocks.Clear();
                        Document.Blocks.AddRange(flowDocument.Blocks.ToList());
                    }

                    _setContentInProgress = false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the content of the RichTextBox is empty / whitespace or not.
        /// </summary>
        public bool IsEmpty => (bool)GetValue(IsEmptyPropertyKey.DependencyProperty);

        private void SetIsEmpty(bool value) => SetValue(IsEmptyPropertyKey, value);

        public BasicTextEditorBox()
        {
            SetIsEmpty(true);

            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;

            DataObject.AddPastingHandler(this, OnPaste);
        }

        public void UpdateContent()
        {
            DocumentContent = FlowDocumentHelper.SerializeDocument(Document);
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
            bool isEmpty = IsRichTextBoxEmpty();
            SetIsEmpty(isEmpty);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // Prevent pasting an image because it cannot be persisted.
            if (e.FormatToApply == DataFormats.Bitmap)
            {
                e.CancelCommand();
            }
        }

        private bool IsRichTextBoxEmpty()
        {
            List<string> res = FlowDocumentHelper.GetDocumentItems(Document);

            return res.Count == 0 || res.All(item => string.IsNullOrWhiteSpace(item));
        }
    }
}
