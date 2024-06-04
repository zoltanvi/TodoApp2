using Modules.Common;
using Modules.Common.DataBinding;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Settings.Contracts.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TodoApp2.Core;
using MediaFontFamily = System.Windows.Media.FontFamily;

namespace TodoApp2;

public class FormattableTextEditorBox : BasicTextEditorBox
{
    private bool _executing;
    private readonly StringRGBToBrushConverter _colorConverter;

    public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata());
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(string), typeof(FormattableTextEditorBox), new PropertyMetadata());
    public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(nameof(TextColor), typeof(string), typeof(FormattableTextEditorBox), new PropertyMetadata(OnTextColorChanged));

    public static readonly DependencyProperty IsFormattedPasteEnabledProperty = DependencyProperty.Register(nameof(IsFormattedPasteEnabled), typeof(bool), typeof(FormattableTextEditorBox), new PropertyMetadata(true));

    public static readonly DependencyProperty EnterActionProperty = DependencyProperty.Register(nameof(EnterAction), typeof(Action), typeof(FormattableTextEditorBox), new PropertyMetadata());
    public static readonly DependencyProperty EmptyContentUpArrowActionProperty = DependencyProperty.Register(nameof(EmptyContentUpArrowAction), typeof(Action), typeof(FormattableTextEditorBox), new PropertyMetadata());

    public event EventHandler StatePropertyChanged;

    public Action CtrlShiftEnterAction { get; set; }

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

    public bool IsFormattedPasteEnabled
    {
        get => (bool)GetValue(IsFormattedPasteEnabledProperty);
        set => SetValue(IsFormattedPasteEnabledProperty, value);
    }

    public Action EnterAction
    {
        get { return (Action)GetValue(EnterActionProperty); }
        set { SetValue(EnterActionProperty, value); }
    }

    public Action EmptyContentUpArrowAction
    {
        get { return (Action)GetValue(EmptyContentUpArrowActionProperty); }
        set { SetValue(EmptyContentUpArrowActionProperty, value); }
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
    public ICommand AlignLeftCommand { get; set; }
    public ICommand AlignCenterCommand { get; set; }
    public ICommand AlignRightCommand { get; set; }
    public ICommand AlignJustifyCommand { get; set; }


    private bool IsCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

    public FormattableTextEditorBox()
    {
        _colorConverter = new StringRGBToBrushConverter();

        CommandManager.AddPreviewExecutedHandler(this, OnExecuted);
        SelectionChanged += OnSelectionChanged;
        PreviewKeyDown += OnPrevKeyDown;
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

        AlignLeftCommand = new RelayCommand(() => EditingCommands.AlignLeft.Execute(null, this));
        AlignCenterCommand = new RelayCommand(() => EditingCommands.AlignCenter.Execute(null, this));
        AlignRightCommand = new RelayCommand(() => EditingCommands.AlignRight.Execute(null, this));
        AlignJustifyCommand = new RelayCommand(() => EditingCommands.AlignJustify.Execute(null, this));

        PreviewKeyDown += FormattableTextEditorBox_PreviewKeyDown;

        DataObject.AddPastingHandler(this, OnPaste);
    }

    private void OnPrevKeyDown(object sender, KeyEventArgs e)
    {
        var ctrl = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
        var shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

        if (ctrl && shift && e.Key != Key.LeftCtrl && e.Key != Key.LeftShift)
        {
            if (e.Key == Key.L)
            {
                e.Handled = false;
            }
        }
    }

    private void FormattableTextEditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var keyArrowUp = e.Key == Key.Up;
        var keyL = e.Key == Key.L;
        var keyN = e.Key == Key.N;
        var enter = e.Key == Key.Enter;
        var escape = e.Key == Key.Escape;
        var shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
        var ctrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

        if (escape || enter && !shiftPressed)
        {
            if (AppSettings.Instance.TaskPageSettings.SaveOnEnter || ctrlPressed && enter)
            {
                UpdateContent();

                var wasEmpty = IsEmpty;

                EnterAction?.Invoke();

                e.Handled = true;

                if (escape && wasEmpty)
                {
                    Keyboard.ClearFocus();
                }
            }
        }

        if (ctrlPressed && shiftPressed && (keyL || keyN))
        {
            e.Handled = true;

            if (keyL)
            {
                Mediator.NotifyClients(ViewModelMessages.NextThemeWithHotkeyRequested);
            }
        }

        if (ctrlPressed && shiftPressed && enter)
        {
            e.Handled = true;
            CtrlShiftEnterAction?.Invoke();
        }

        if (keyArrowUp && IsEmpty)
        {
            e.Handled = true;
            EmptyContentUpArrowAction?.Invoke();
        }
    }

    private void ChangeToMonospace()
    {
        var currentFontFamily = Selection.GetPropertyValue(TextElement.FontFamilyProperty);

        if (currentFontFamily is MediaFontFamily fontFamily && fontFamily.Source == Constants.FontFamily.Consolas)
        {
            MediaFontFamily defaultFontFamily = EnumToFontFamilyConverter.Instance.Convert(AppSettings.Instance.TaskSettings.FontFamily);

            if (defaultFontFamily != null)
            {
                Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, defaultFontFamily);
            }
        }
        else
        {
            MediaFontFamily consolas = EnumToFontFamilyConverter.Instance.Convert(Modules.Common.DataModels.FontFamily.Consolas);

            if (consolas != null)
            {
                Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, consolas);
            }
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
        var fontSize = Selection.GetPropertyValue(TextElement.FontSizeProperty);
        if (fontSize is double size && size > 2)
        {
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size - 2);
        }
    }

    private void IncreaseFontSize()
    {
        var fontSize = Selection.GetPropertyValue(TextElement.FontSizeProperty);
        if (fontSize is double size)
        {
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size + 2);
        }
    }

    private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        // First execute the command then do the update
        if (!_executing)
        {
            _executing = true;

            try
            {
                e.Command?.Execute(e.Parameter);
                e.Handled = true;
            }
            catch (FormatException)
            {
                e.Handled = false;
            }

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

            _executing = false;
        }
    }

    private void UpdateSelectionBold()
    {
        var fontWeight = Selection.GetPropertyValue(TextElement.FontWeightProperty);
        IsSelectionBold = fontWeight != DependencyProperty.UnsetValue && fontWeight.Equals(FontWeights.Bold);
    }

    private void UpdateSelectionItalic()
    {
        var fontStyle = Selection.GetPropertyValue(TextElement.FontStyleProperty);
        IsSelectionItalic = fontStyle != DependencyProperty.UnsetValue && fontStyle.Equals(FontStyles.Italic);
    }

    private void UpdateSelectionUnderlined()
    {
        var decorations = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
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
        var foreground = Selection.GetPropertyValue(TextElement.ForegroundProperty);
        var color = Constants.ColorName.Transparent;

        if (foreground != DependencyProperty.UnsetValue)
        {
            color = (string)_colorConverter.ConvertBack(foreground, typeof(string), null, CultureInfo.InvariantCulture);
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

        StatePropertyChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ResetFormatting()
    {
        SelectAll();
        Selection.ClearAllProperties();
        UpdateContent();
    }

    private void ApplyColor(string newColor)
    {
        var color = _colorConverter.Convert(
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
            var defaultColor = (SolidColorBrush)Application.Current.TryFindResource(Constants.BrushName.ForegroundBrush);
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

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (!IsFormattedPasteEnabled)
        {
            e.DataObject = new DataObject(DataFormats.Text, e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty);
        }
    }
}
