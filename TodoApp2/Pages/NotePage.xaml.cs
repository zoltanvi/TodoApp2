using Modules.Common.Navigation;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.Views.Pages;
using System;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for NotePage.xaml
    /// </summary>
    public partial class NotePage : GenericBasePage<NotePageViewModel>, INoteEditorPage
    {
        public NotePage(NotePageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();

            textEditor.Text = IoC.NoteListService.ActiveNote?.Content ?? string.Empty;
            textEditor.Options.HighlightCurrentLine = true;
            textEditor.TextArea.SelectionCornerRadius = 2;
            textEditor.Options.EnableHyperlinks = false;
            textEditor.Options.CutCopyWholeLine = true;
            textEditor.Options.AllowScrollBelowDocument = true;

            textEditor.PreviewKeyDown += OnTextEditorPreviewKeyDown;

            Mediator.Register(OnNoteChanged, ViewModelMessages.NoteChanged);
        }

        private void OnNoteChanged(object obj)
        {
            textEditor.Text = IoC.NoteListService.ActiveNote?.Content ?? string.Empty;
        }

        private bool IsTab(Key key) => key == Key.Tab;
        private bool IsSKey(Key key) => key == Key.S;
        private bool IsUpOrDown(Key key) => key == Key.Up || key == Key.Down;
        private bool IsLeftOrRight(Key key) => key == Key.Left || key == Key.Right;
        private bool IsCtrlPressed => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
        private bool IsAltPressed => Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

        private void OnTextEditorPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;

            if (IsUpOrDown(key) && IsCtrlPressed)
            {
                e.Handled = true;
            }
            else if (IsLeftOrRight(key) && IsCtrlPressed && IsAltPressed)
            {
                e.Handled = true;
            }
            else if (IsTab(key) && IsCtrlPressed)
            {
                e.Handled = true;
            }
            else if (IsSKey(key) && IsCtrlPressed)
            {
                ViewModel.SaveNoteContent();
            }
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            if (IoC.NoteListService.ActiveNote != null)
            {
                IoC.NoteListService.ActiveNote.Content = textEditor.Text;
                ViewModel.NoteContentChanged();
            }
        }
    }
}
