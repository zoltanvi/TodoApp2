using System.Windows.Input;

namespace TodoApp2.Core
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly AppViewModel _AppViewModel;

        public ApplicationPage ActiveSettingsPage { get; set; } = ApplicationPage.WindowSettings;

        public ICommand GoBackCommand { get; }
        public ICommand OpenWindowSettingsCommand { get; }
        public ICommand OpenThemeSettingsCommand { get; }
        public ICommand OpenPageTitleSettingsCommand { get; }
        public ICommand OpenTaskPageSettingsCommand { get; }
        public ICommand OpenTaskItemSettingsCommand { get; }
        public ICommand OpenQuickActionSettingsCommand { get; }
        public ICommand OpenTextEditorQuickActionSettingsCommand { get; }
        public ICommand OpenNoteSettingsCommand { get; }
        public ICommand OpenThemeEditorSettingsCommand { get; }
        
        public SettingsPageViewModel()
        {
        }

        public SettingsPageViewModel(AppViewModel applicationViewModel)
        {
            _AppViewModel = applicationViewModel;            
            GoBackCommand = new RelayCommand(() => _AppViewModel.UpdateMainPage());

            OpenWindowSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.WindowSettings);
            OpenThemeSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.ThemeSettings);
            OpenPageTitleSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.PageTitleSettings);
            OpenTaskPageSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.TaskPageSettings);
            OpenTaskItemSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.TaskItemSettings);
            OpenQuickActionSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.TaskQuickActionsSettings);
            OpenTextEditorQuickActionSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.TextEditorQuickActionsSettings);
            OpenNoteSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.NotePageSettings);
            OpenThemeEditorSettingsCommand = new RelayCommand(() => ActiveSettingsPage = ApplicationPage.ThemeEditorSettings);
        }
    }
}
