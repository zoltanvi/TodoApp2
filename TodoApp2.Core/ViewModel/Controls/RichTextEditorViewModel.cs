using System.Windows.Input;

namespace TodoApp2.Core
{
    public class RichTextEditorViewModel : BaseViewModel
    {
        public string DocumentContent { get; set; }
        public bool IsToolbarOpen { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsDisplayMode => !IsEditMode;

        public string TextColor { get; set; }

        public ICommand EndEditModeCommand { get; }

        public RichTextEditorViewModel()
        {
            EndEditModeCommand = new RelayCommand(() =>
            {
                IsToolbarOpen = false;
                IsEditMode = false;
            });
        }
    }
}
