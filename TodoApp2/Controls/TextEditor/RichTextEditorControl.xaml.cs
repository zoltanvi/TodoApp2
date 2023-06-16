using System;
using System.Windows;
using System.Windows.Controls;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for RichTextEditorControl.xaml
    /// </summary>
    public partial class RichTextEditorControl : UserControl
    {
        private SingletonToolbar _toolbar;

        public static readonly DependencyProperty TextOpacityProperty = DependencyProperty.Register(nameof(TextOpacity), typeof(double), typeof(RichTextEditorControl), new PropertyMetadata());
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RichTextEditorControl), new PropertyMetadata());

        public double TextOpacity
        {
            get { return (double)GetValue(TextOpacityProperty); }
            set { SetValue(TextOpacityProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public RichTextEditorControl()
        {
            InitializeComponent();
            IsEditorOpenToggle.Checked += IsEditorOpenToggle_Checked;
            IsEditorOpenToggle.Unchecked += IsEditorOpenToggle_Unchecked;
            PART_TextEditor.CtrlShiftEnterAction = ToggleEditorOpen;
        }

        private void IsEditorOpenToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (ToolBarPanel.Children.Count == 0)
            {
                if (_toolbar == null)
                {
                    _toolbar = ToolbarService.Toolbar;
                }

                _toolbar.SetParentStackPanel(ToolBarPanel);

                SetToolbarActions(_toolbar);

                PART_TextEditor.StatePropertyChanged += OnTextEditorStatePropertyChanged;
            }
        }

        private void ToggleEditorOpen()
        {
            IsEditorOpenToggle.IsChecked = !IsEditorOpenToggle.IsChecked;
        }

        private void IsEditorOpenToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ToolBarPanel.Children.Count != 0)
            {
                ToolBarPanel.Children.Clear();
                PART_TextEditor.StatePropertyChanged -= OnTextEditorStatePropertyChanged;
            }
        }

        private void SetToolbarActions(SingletonToolbar toolbar)
        {
            toolbar.SetBoldCommandAction(() => PART_TextEditor.SetBoldCommand?.Execute(null));
            toolbar.SetItalicCommandAction(() => PART_TextEditor.SetItalicCommand?.Execute(null));
            toolbar.SetUnderlinedCommandAction(() => PART_TextEditor.SetUnderlinedCommand?.Execute(null));
            toolbar.SetSmallFontSizeCommandAction(() => PART_TextEditor.SetSmallFontSizeCommand?.Execute(null));
            toolbar.SetMediumFontSizeCommandAction(() => PART_TextEditor.SetMediumFontSizeCommand?.Execute(null));
            toolbar.SetBigFontSizeCommandAction(() => PART_TextEditor.SetBigFontSizeCommand?.Execute(null));
            toolbar.IncreaseFontSizeCommandAction(() => PART_TextEditor.IncreaseFontSizeCommand?.Execute(null));
            toolbar.DecreaseFontSizeCommandAction(() => PART_TextEditor.DecreaseFontSizeCommand?.Execute(null));
            toolbar.ResetFormattingCommandAction(() => PART_TextEditor.ResetFormattingCommand?.Execute(null));
            toolbar.MonospaceCommandAction(() => PART_TextEditor.MonospaceCommand?.Execute(null));
            toolbar.AlignLeftCommandAction(() => PART_TextEditor.AlignLeftCommand?.Execute(null));
            toolbar.AlignCenterCommandAction(() => PART_TextEditor.AlignCenterCommand?.Execute(null));
            toolbar.AlignRightCommandAction(() => PART_TextEditor.AlignRightCommand?.Execute(null));
            toolbar.AlignJustifyCommandAction(() => PART_TextEditor.AlignJustifyCommand?.Execute(null));
        }

        private void OnTextEditorStatePropertyChanged(object sender, System.EventArgs e)
        {
            _toolbar.IsSelectionBold = PART_TextEditor.IsSelectionBold;
            _toolbar.IsSelectionItalic = PART_TextEditor.IsSelectionItalic;
            _toolbar.IsSelectionUnderlined = PART_TextEditor.IsSelectionUnderlined;
        }
    }
}
