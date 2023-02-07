using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TodoApp2.Core;
using TodoApp2.Core.Constants;
using TodoApp2.Helpers;

namespace TodoApp2.Adorners
{
    public class TextBoxCurrentLineAdorner : Adorner
    {
        private Brush m_HighlightBrush;
        private TextBox m_TextBox;
        private Rect m_CachedRect;

        public TextBoxCurrentLineAdorner(TextBox textBox, UIElement adornedElement) : base(adornedElement)
        {
            m_HighlightBrush = (SolidColorBrush)Application.Current.TryFindResource(
                GlobalConstants.BrushName.NoteLineAdornerBgBrush);
            m_TextBox = textBox;
            m_TextBox.SelectionChanged += OnTextBoxSelectionChanged;
            m_TextBox.SizeChanged += OnTextBoxSizeChanged;
            IsHitTestVisible = false;
            m_CachedRect = Rect.Empty;

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
            m_HighlightBrush = (SolidColorBrush)Application.Current.TryFindResource(
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

            int caretIndex = m_TextBox.CaretIndex;
            int textLength = m_TextBox.Text.Length;
            string text = m_TextBox.Text;

            double characterHeight = TextBoxHelper.GetTextHeight(m_TextBox, "a");

            int lineNumberStart = m_TextBox.GetLineIndexFromCharacterIndex(caretIndex);
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

                    lineNumberStart = m_TextBox.GetLineIndexFromCharacterIndex(lineStartCharIndex);
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

                    lineNumberEnd = m_TextBox.GetLineIndexFromCharacterIndex(lineEndCharIndex);
                }
            }

            Point lineStart = new Point(0, m_TextBox.Padding.Top + (lineNumberStart * characterHeight));
            Point lineEnd = new Point(m_TextBox.ActualWidth, m_TextBox.Padding.Top + ((lineNumberEnd + 1) * characterHeight));

            m_CachedRect = new Rect(lineStart, lineEnd);
            drawingContext.DrawRectangle(m_HighlightBrush, null, m_CachedRect);
        }
    }
}
