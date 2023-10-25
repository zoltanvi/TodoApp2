using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TodoApp2.Core;
using TodoApp2.Helpers;

namespace TodoApp2.Adorners
{
    public class TextBoxCurrentLineAdorner : Adorner
    {
        private Brush _highlightBrush;
        private TextBox _textBox;
        private Rect _cachedRect;

        public TextBoxCurrentLineAdorner(TextBox textBox, UIElement adornedElement) : base(adornedElement)
        {
            _highlightBrush = (SolidColorBrush)Application.Current.TryFindResource(
                GlobalConstants.BrushName.NoteLineAdornerBgBrush);
            _textBox = textBox;
            _textBox.SelectionChanged += OnTextBoxSelectionChanged;
            _textBox.SizeChanged += OnTextBoxSizeChanged;
            IsHitTestVisible = false;
            _cachedRect = Rect.Empty;

            // Subscribe to the theme changed event to repaint the list items when it happens
            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);

            IoC.UIScaler.Zoomed += OnUiScalerZoomed;
        }

        private void OnUiScalerZoomed(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void OnThemeChanged(object obj)
        {
            _highlightBrush = (SolidColorBrush)Application.Current.TryFindResource(
                GlobalConstants.BrushName.NoteLineAdornerBgBrush);
            InvalidateVisual();
        }

        private void OnTextBoxSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            int caretIndex = _textBox.CaretIndex;
            int textLength = _textBox.Text.Length;
            string text = _textBox.Text;

            double characterHeight = TextBoxHelper.GetTextHeight(_textBox, "a");

            int lineNumberStart = _textBox.GetLineIndexFromCharacterIndex(caretIndex);
            int lineNumberEnd = lineNumberStart;

            if (textLength == 0)
            {
                lineNumberEnd = 0;
            }
            else
            {
                if (caretIndex != 0)
                {
                    // Look backward
                    int lineStartCharIndex = caretIndex;
                    for (int i = caretIndex - 1; i >= 0; i--)
                    {
                        if (text[i] == '\n')
                        {
                            break;
                        }

                        lineStartCharIndex = i;
                    }

                    lineNumberStart = _textBox.GetLineIndexFromCharacterIndex(lineStartCharIndex);
                }

                // NOT standing on last position
                // Look forward
                int lineEndCharIndex = caretIndex;
                if (caretIndex != textLength)
                {
                    for (int i = caretIndex; i < textLength; i++)
                    {
                        if (text[i] == '\n')
                        {
                            break;
                        }

                        lineEndCharIndex = i;
                    }

                    lineNumberEnd = _textBox.GetLineIndexFromCharacterIndex(lineEndCharIndex);
                }
            }

            Point lineStart = new Point(0, _textBox.Padding.Top + (lineNumberStart * characterHeight));
            Point lineEnd = new Point(_textBox.ActualWidth, _textBox.Padding.Top + ((lineNumberEnd + 1) * characterHeight));

            _cachedRect = new Rect(lineStart, lineEnd);
            drawingContext.DrawRectangle(_highlightBrush, null, _cachedRect);
        }
    }
}
