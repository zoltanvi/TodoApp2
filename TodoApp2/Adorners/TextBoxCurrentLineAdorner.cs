using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TodoApp2.Adorners
{
    public class TextBoxCurrentLineAdorner : Adorner
    {
        private Brush m_HighlightBrush = new SolidColorBrush(Color.FromArgb(64, 213, 213, 213));
        private TextBox m_TextBox;
        private Rect m_CachedRect;

        public TextBoxCurrentLineAdorner(TextBox textBox) : base(textBox)
        {
            m_TextBox = textBox;
            m_TextBox.SelectionChanged += OnTextBoxSelectionChanged;
            m_TextBox.SizeChanged += OnTextBoxSizeChanged;
            IsHitTestVisible = false;
            m_CachedRect = Rect.Empty;
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
            string character = "a";

            Typeface typeFace = new Typeface(m_TextBox.FontFamily, m_TextBox.FontStyle, m_TextBox.FontWeight, m_TextBox.FontStretch);
            FormattedText formattedText = new FormattedText(
                character,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeFace,
                m_TextBox.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double characterHeight = formattedText.Height;

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
