using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace TodoApp2
{
    public class BindableRichTextBox : RichTextBox
    {
        private static readonly string s_EmptyContent = XamlWriter.Save(new FlowDocument());
        private static readonly IEnumerable<Block> s_EmptyBlocks = new FlowDocument().Blocks;

        private bool m_IsExecuting;

        /// <summary>
        /// The Document of the <see cref="BindableRichTextBox"/> serialized into an xml format.
        /// </summary>
        public static readonly DependencyProperty DocumentContentProperty = DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(BindableRichTextBox), new PropertyMetadata(OnContentChanged));

        /// <summary>
        /// Indicates whether the <see cref="BindableRichTextBox"/> is empty or not.
        /// If the content is only whitespace, it returns true also.
        /// </summary>
        public static readonly DependencyProperty IsEmptyOrWhiteSpaceProperty = DependencyProperty.Register(nameof(IsEmptyOrWhiteSpace), typeof(bool), typeof(BindableRichTextBox), new PropertyMetadata());

        /// <summary>
        /// Indicates whether the <see cref="BindableRichTextBox"/> is completely empty or not.
        /// </summary>
        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(BindableRichTextBox), new PropertyMetadata());

        public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(BindableRichTextBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(BindableRichTextBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(BindableRichTextBox), new PropertyMetadata());

        /// <see cref="DocumentContentProperty"/>
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

        /// <see cref="IsEmptyOrWhiteSpaceProperty"/>
        public bool IsEmptyOrWhiteSpace
        {
            get => (bool)GetValue(IsEmptyOrWhiteSpaceProperty);
            set => SetValue(IsEmptyOrWhiteSpaceProperty, value);
        }

        /// <see cref="IsEmptyProperty"/>
        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValue(IsEmptyProperty, value);
        }

        public bool IsSelectionBold
        {
            get => (bool)GetValue(IsSelectionBoldProperty);
            set => SetValue(IsSelectionBoldProperty, value);
        }

        public bool IsSelectionItalic
        {
            get => (bool)GetValue(IsSelectionItalicProperty);
            set => SetValue(IsSelectionItalicProperty, value);
        }

        public bool IsSelectionUnderlined
        {
            get => (bool)GetValue(IsSelectionUnderlinedProperty);
            set => SetValue(IsSelectionUnderlinedProperty, value);
        }

        public BindableRichTextBox()
        {
            IsEmpty = true;
            IsEmptyOrWhiteSpace = true;

            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;
            KeyDown += OnKeyDown;
            SelectionChanged += OnSelectionChanged;

            DataObject.AddPastingHandler(this, OnPaste);
            CommandManager.AddPreviewExecutedHandler(this, OnExecuted);
        }

        private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!m_IsExecuting)
            {
                m_IsExecuting = true;

                e.Command?.Execute(e.Parameter);
                e.Handled = true;
                
                if (e.Command == EditingCommands.ToggleBold)
                {
                    UpdateSelectionBold();
                }
                else if (e.Command == EditingCommands.ToggleItalic)
                {
                    UpdateSelectionItalic();
                }
                else if (e.Command == EditingCommands.ToggleUnderline)
                {
                    UpdateSelectionUnderlined();
                }

                m_IsExecuting = false;
            }
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

        private void UpdateSelectionBold()
        {
            object fontWeight = Selection.GetPropertyValue(TextElement.FontWeightProperty);
            IsSelectionBold = fontWeight != DependencyProperty.UnsetValue && fontWeight.Equals(FontWeights.Bold);
        }

        private void UpdateSelectionItalic()
        {
            object fontStyle = Selection.GetPropertyValue(TextElement.FontStyleProperty);
            IsSelectionItalic = fontStyle != DependencyProperty.UnsetValue && fontStyle.Equals(FontStyles.Italic);
        }

        private void UpdateSelectionUnderlined()
        {
            object decorations = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            IsSelectionUnderlined = decorations != DependencyProperty.UnsetValue && decorations.Equals(TextDecorations.Underline);
        }


        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectionBold();
            UpdateSelectionItalic();
            UpdateSelectionUnderlined();

            //var tmp = Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            //tmp = Selection.GetPropertyValue(TextElement.FontSizeProperty);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // TODO: Implement Shift + TAB

            // Delete text formatting on Ctrl + G
            if (e.Key == Key.G && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                TextRange textRange = new TextRange(Document.ContentStart, Document.ContentEnd);
                string[] documentLines = textRange.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                Document.Blocks.Clear();

                foreach (string documentLine in documentLines)
                {
                    Document.Blocks.Add(new Paragraph(new Run(documentLine)));
                }

                UpdateContent();
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // Prevent pasting an image because it cannot be persisted.
            if (e.FormatToApply == "Bitmap")
            {
                e.CancelCommand();
            }
        }


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            IsEmpty = IsRichTextBoxEmpty(this);
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
