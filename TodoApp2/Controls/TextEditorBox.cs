using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    public class TextEditorBox : RichTextBox
    {
        private static readonly string s_EmptyContent = XamlWriter.Save(new FlowDocument());
        private static readonly IEnumerable<Block> s_EmptyBlocks = new FlowDocument().Blocks;

        private readonly StringRGBToBrushConverter m_ColorConverter;
        private bool m_IsExecuting;

        /// <summary>
        /// The Document of the <see cref="TextEditorBox"/> serialized into an xml format.
        /// </summary>
        public static readonly DependencyProperty DocumentContentProperty = DependencyProperty.Register(nameof(DocumentContent), typeof(string), typeof(TextEditorBox), new PropertyMetadata(OnContentChanged));

        public static readonly DependencyProperty IsEmptyOrWhiteSpaceProperty = DependencyProperty.Register(nameof(IsEmptyOrWhiteSpace), typeof(bool), typeof(TextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(nameof(IsEmpty), typeof(bool), typeof(TextEditorBox), new PropertyMetadata());

        public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(TextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(TextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(TextEditorBox), new PropertyMetadata());

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(string), typeof(TextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty AppliedColorProperty = DependencyProperty.Register(nameof(AppliedColor), typeof(string), typeof(TextEditorBox), new PropertyMetadata(OnAppliedColorChanged));

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

        public bool IsEmptyOrWhiteSpace
        {
            get => (bool)GetValue(IsEmptyOrWhiteSpaceProperty);
            set => SetValue(IsEmptyOrWhiteSpaceProperty, value);
        }

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

        public string SelectedColor
        {
            get => (string)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public string AppliedColor
        {
            get => (string)GetValue(AppliedColorProperty);
            set => SetValue(AppliedColorProperty, value);
        }

        public ICommand ResetFormattingCommand { get; set; }

        public TextEditorBox()
        {
            m_ColorConverter = new StringRGBToBrushConverter();

            IsEmpty = true;
            IsEmptyOrWhiteSpace = true;

            LostFocus += OnLostFocus;
            TextChanged += OnTextChanged;
            KeyDown += OnKeyDown;
            SelectionChanged += OnSelectionChanged;

            DataObject.AddPastingHandler(this, OnPaste);
            CommandManager.AddPreviewExecutedHandler(this, OnExecuted);

            ResetFormattingCommand = new RelayCommand(ResetFormatting);
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

        private void ResetFormatting()
        {
            Selection.ClearAllProperties();
            UpdateContent();
        }

        private static void OnAppliedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextEditorBox textEditorBox && e.NewValue is string newColor)
            {
                textEditorBox.ApplyColor(newColor);
            }
        }

        private void ApplyColor(string newColor)
        {
            SolidColorBrush color = m_ColorConverter.Convert(newColor, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture) as SolidColorBrush;
            if (color?.Color.A != 0)
            {
                Selection.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            }
            else
            {
                SolidColorBrush defaultColor = (SolidColorBrush)Application.Current.TryFindResource("TaskPageForegroundBrush");
                Selection.ApplyPropertyValue(TextElement.ForegroundProperty, defaultColor);
            }
        }

        private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // First execute the command then do the update
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
            IsSelectionUnderlined = false;

            if (decorations != DependencyProperty.UnsetValue && decorations is TextDecorationCollection decorationCollection)
            {
                foreach (TextDecoration textDecoration in decorationCollection)
                {
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                    {
                        IsSelectionUnderlined = true;
                        break;
                    }
                    //else if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                    //{
                    //}
                }
            }
        }

        private void UpdateSelectionColor()
        {
            string oldValue = SelectedColor;
            object foreground = Selection.GetPropertyValue(TextElement.ForegroundProperty);
            string color = "Transparent";

            if (foreground != DependencyProperty.UnsetValue)
            {
                color = (string)m_ColorConverter.ConvertBack(foreground, typeof(string), null, CultureInfo.InvariantCulture);
            }

            SelectedColor = color;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectionBold();
            UpdateSelectionItalic();
            UpdateSelectionUnderlined();

            UpdateSelectionColor();

            //var tmp = Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            //tmp = Selection.GetPropertyValue(TextElement.FontSizeProperty);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // TODO: Implement Shift + TAB

            // Delete text formatting on Ctrl + H
            if (e.Key == Key.H && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ResetFormatting();
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
            if (d is TextEditorBox textEditorBox &&
                e.NewValue is string newContent)
            {
                textEditorBox.DocumentContent = newContent;
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
