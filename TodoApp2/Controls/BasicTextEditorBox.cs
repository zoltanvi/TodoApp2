using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using TodoApp2.Core;
using TodoApp2.Helpers;

namespace TodoApp2
{
    public class BasicTextEditorBox : RichTextBox
    {
        internal static readonly DependencyPropertyKey IsEmptyPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsEmpty), typeof(bool), typeof(BasicTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty DocumentContentProperty = DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(BasicTextEditorBox), new PropertyMetadata(OnContentChanged));

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
                    Document.Blocks.Clear();
                    Document.Blocks.AddRange(FlowDocumentHelper.EmptyFlowDocumentBlocks);
                }
                else
                {
                    if (FlowDocumentHelper.DeserializeDocument(value) is FlowDocument flowDocument)
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
            bool isEmpty = IsRichTextBoxEmpty(this);
            SetIsEmpty(isEmpty);
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
