using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2;

public class SingletonToolbar : Label
{
    public static readonly DependencyProperty IsSelectionBoldProperty = DependencyProperty.Register(nameof(IsSelectionBold), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionItalicProperty = DependencyProperty.Register(nameof(IsSelectionItalic), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());
    public static readonly DependencyProperty IsSelectionUnderlinedProperty = DependencyProperty.Register(nameof(IsSelectionUnderlined), typeof(bool), typeof(SingletonToolbar), new PropertyMetadata());

    public StackPanel ParentStackPanel { get; private set; }

    public bool IsSelectionBold
    {
        get { return (bool)GetValue(IsSelectionBoldProperty); }
        set { SetValue(IsSelectionBoldProperty, value); }
    }

    public bool IsSelectionItalic
    {
        get { return (bool)GetValue(IsSelectionItalicProperty); }
        set { SetValue(IsSelectionItalicProperty, value); }
    }
    public bool IsSelectionUnderlined
    {
        get { return (bool)GetValue(IsSelectionUnderlinedProperty); }
        set { SetValue(IsSelectionUnderlinedProperty, value); }
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

    public SingletonToolbar()
    {
        SetBoldCommand = new DynamicRelayCommand();
        SetItalicCommand = new DynamicRelayCommand();
        SetUnderlinedCommand = new DynamicRelayCommand();
        SetSmallFontSizeCommand = new DynamicRelayCommand();
        SetMediumFontSizeCommand = new DynamicRelayCommand();
        SetBigFontSizeCommand = new DynamicRelayCommand();
        IncreaseFontSizeCommand = new DynamicRelayCommand();
        DecreaseFontSizeCommand = new DynamicRelayCommand();
        ResetFormattingCommand = new DynamicRelayCommand();
        MonospaceCommand = new DynamicRelayCommand();
        AlignLeftCommand = new DynamicRelayCommand();
        AlignCenterCommand = new DynamicRelayCommand();
        AlignRightCommand = new DynamicRelayCommand();
        AlignJustifyCommand = new DynamicRelayCommand();
    }

    public void SetParentStackPanel(StackPanel stackPanel)
    {
        // Remove from previous parent
        if (ParentStackPanel != null)
        {
            ParentStackPanel.Children.Clear();
        }

        ParentStackPanel = stackPanel;
        ParentStackPanel.Children.Add(this);
    }

    public void SetBoldCommandAction(Action action) => ((DynamicRelayCommand)SetBoldCommand).SetAction(action);
    public void SetItalicCommandAction(Action action) => ((DynamicRelayCommand)SetItalicCommand).SetAction(action);
    public void SetUnderlinedCommandAction(Action action) => ((DynamicRelayCommand)SetUnderlinedCommand).SetAction(action);
    public void SetSmallFontSizeCommandAction(Action action) => ((DynamicRelayCommand)SetSmallFontSizeCommand).SetAction(action);
    public void SetMediumFontSizeCommandAction(Action action) => ((DynamicRelayCommand)SetMediumFontSizeCommand).SetAction(action);
    public void SetBigFontSizeCommandAction(Action action) => ((DynamicRelayCommand)SetBigFontSizeCommand).SetAction(action);
    public void IncreaseFontSizeCommandAction(Action action) => ((DynamicRelayCommand)IncreaseFontSizeCommand).SetAction(action);
    public void DecreaseFontSizeCommandAction(Action action) => ((DynamicRelayCommand)DecreaseFontSizeCommand).SetAction(action);
    public void ResetFormattingCommandAction(Action action) => ((DynamicRelayCommand)ResetFormattingCommand).SetAction(action);
    public void MonospaceCommandAction(Action action) => ((DynamicRelayCommand)MonospaceCommand).SetAction(action);
    public void AlignLeftCommandAction(Action action) => ((DynamicRelayCommand)AlignLeftCommand).SetAction(action);
    public void AlignCenterCommandAction(Action action) => ((DynamicRelayCommand)AlignCenterCommand).SetAction(action);
    public void AlignRightCommandAction(Action action) => ((DynamicRelayCommand)AlignRightCommand).SetAction(action);
    public void AlignJustifyCommandAction(Action action) => ((DynamicRelayCommand)AlignJustifyCommand).SetAction(action);
}
