using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for NotePage.xaml
    /// </summary>
    public partial class NotePage : BasePage<NotePageViewModel>
    {
        public NotePage(NotePageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();

            textEditor.Text = IoC.ApplicationViewModel.ApplicationSettings.NoteContent;
            textEditor.Options.HighlightCurrentLine = true;
            textEditor.TextArea.SelectionCornerRadius = 2;

            textEditor.PreviewKeyDown += OnTextEditorPreviewKeyDown;
        }

        private void OnTextEditorPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ViewModel.SaveNoteContent();
            }
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            IoC.ApplicationViewModel.ApplicationSettings.NoteContent = textEditor.Text;
        }
    }
}
