using System.Windows;
using System.Windows.Controls;

namespace Modules.Notes.Views.Controls;

/// <summary>
/// Interaction logic for NoteListItemControl.xaml
/// </summary>
public partial class NoteListItemControl : UserControl
{
    public static readonly DependencyProperty ActiveNoteIdProperty =
        DependencyProperty.Register(nameof(ActiveNoteId), typeof(int), typeof(NoteListItemControl), new PropertyMetadata(-1));

    public int ActiveNoteId
    {
        get { return (int)GetValue(ActiveNoteIdProperty); }
        set { SetValue(ActiveNoteIdProperty, value); }
    }

    public NoteListItemControl()
    {
        InitializeComponent();
    }
}