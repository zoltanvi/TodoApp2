using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TodoApp2.Core;
using TodoApp2.Core.Constants;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace TodoApp2
{
    internal class FormattableTextEditorBox : BasicTextEditorBox
    {
        private bool m_IsExecuting;
        private readonly StringRGBToBrushConverter m_ColorConverter;

        public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(string), typeof(FormattableTextEditorBox), new PropertyMetadata());
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(nameof(TextColor), typeof(string), typeof(FormattableTextEditorBox), new PropertyMetadata(OnTextColorChanged));

        public static readonly DependencyProperty EnterActionProperty = DependencyProperty.Register(nameof(EnterAction), typeof(Action), typeof(FormattableTextEditorBox), new PropertyMetadata());

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

        public string TextColor
        {
            get => (string)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Action EnterAction
        {
            get { return (Action)GetValue(EnterActionProperty); }
            set { SetValue(EnterActionProperty, value); }
        }

        public ICommand SetBoldCommand { get; set; }
        public ICommand SetItalicCommand { get; set; }
        public ICommand SetUnderlinedCommand { get; set; }

        public ICommand SetSmallFontSizeCommand { get; set; }
        public ICommand SetMediumFontSizeCommand { get; set; }
        public ICommand SetBigFontSizeCommand { get; set; }

        public ICommand IncreaseFontSizeCommand { get; set; }
        public ICommand DecreaseFontSizeCommand { get; set; }

        public ICommand ResetFormattingCommand { get; set; }

        public ICommand MonospaceCommand { get; set; }

        private bool IsCtrlDown => (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));

        public FormattableTextEditorBox()
        {
            m_ColorConverter = new StringRGBToBrushConverter();

            CommandManager.AddPreviewExecutedHandler(this, OnExecuted);
            SelectionChanged += OnSelectionChanged;
            PreviewKeyUp += OnPrevKeyUp;
            KeyDown += OnKeyDown;

            SetBoldCommand = new RelayCommand(() => EditingCommands.ToggleBold.Execute(null, this));
            SetItalicCommand = new RelayCommand(() => EditingCommands.ToggleItalic.Execute(null, this));
            SetUnderlinedCommand = new RelayCommand(() => EditingCommands.ToggleUnderline.Execute(null, this));

            SetSmallFontSizeCommand = new RelayCommand(() =>
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, IoC.UIScaler.FontSize.Smaller));

            SetMediumFontSizeCommand = new RelayCommand(() =>
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, IoC.UIScaler.FontSize.Medium));

            SetBigFontSizeCommand = new RelayCommand(() =>
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, IoC.UIScaler.FontSize.Huge));

            IncreaseFontSizeCommand = new RelayCommand(IncreaseFontSize);
            DecreaseFontSizeCommand = new RelayCommand(DecreaseFontSize);
            ResetFormattingCommand = new RelayCommand(ResetAllFormatting);
            MonospaceCommand = new RelayCommand(ChangeToMonospace);

            PreviewKeyDown += FormattableTextEditorBox_PreviewKeyDown;
        }

        private void FormattableTextEditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool enter = e.Key == Key.Enter;
            bool escape = e.Key == Key.Escape;
            bool shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            if (escape || (enter && !shiftPressed))
            {
                UpdateContent();

                EnterAction?.Invoke();

                // Mark the key as handled
                e.Handled = true;
            }
        }

        private void ChangeToMonospace()
        {
            MediaFontFamily consolas = new MediaFontFamily(GlobalConstants.FontFamily.Consolas);
            if (consolas != null)
            {
                Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, consolas);
            }
        }

        private void ResetAllFormatting()
        {
            SelectAll();
            Selection.ClearAllProperties();
            UpdateContent();
        }

        private void DecreaseFontSize()
        {
            object fontSize = Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize is double size && size > 2)
            {
                Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size - 2);
            }
        }

        private void IncreaseFontSize()
        {
            object fontSize = Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize is double size)
            {
                Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size + 2);
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
                else if (e.Command == EditingCommands.IncreaseFontSize)
                {
                    IncreaseFontSize();
                    e.Handled = true;
                }
                else if (e.Command == EditingCommands.DecreaseFontSize)
                {
                    DecreaseFontSize();
                    e.Handled = true;
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
            object foreground = Selection.GetPropertyValue(TextElement.ForegroundProperty);
            string color = GlobalConstants.ColorName.Transparent;

            if (foreground != DependencyProperty.UnsetValue)
            {
                color = (string)m_ColorConverter.ConvertBack(foreground, typeof(string), null, CultureInfo.InvariantCulture);
            }

            SelectedColor = color;
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectionProperties();
        }

        private void OnPrevKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                UpdateSelectionProperties();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (IsReadOnly) return;

            // TODO: Implement Shift + TAB
            
            switch (e.Key)
            {
                // Delete text formatting on Ctrl + H
                case Key.H:
                {
                    if (IsCtrlDown)
                    {
                        ResetFormatting();
                    }
                    break;
                }
                // Apply text color on Ctrl + G
                case Key.G:
                {
                    if (IsCtrlDown)
                    {
                        ApplyColor(TextColor);
                    }
                    break;
                }
            }
        }

        private void UpdateSelectionProperties()
        {
            UpdateSelectionBold();
            UpdateSelectionItalic();
            UpdateSelectionUnderlined();

            UpdateSelectionColor();
        }

        private void ResetFormatting()
        {
            SelectAll();
            Selection.ClearAllProperties();
            UpdateContent();
        }

        private void ApplyColor(string newColor)
        {
            SolidColorBrush color = m_ColorConverter.Convert(
                newColor,
                typeof(SolidColorBrush),
                null,
                CultureInfo.InvariantCulture) as SolidColorBrush;

            if (color?.Color.A != 0)
            {
                Selection.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            }
            else
            {
                SolidColorBrush defaultColor = (SolidColorBrush)Application.Current.TryFindResource("ForegroundBrush");
                Selection.ApplyPropertyValue(TextElement.ForegroundProperty, defaultColor);
            }
        }

        private static void OnTextColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormattableTextEditorBox textEditor && e.NewValue is string textColor)
            {
                textEditor.ApplyColor(textColor);
            }
        }
    }
}
