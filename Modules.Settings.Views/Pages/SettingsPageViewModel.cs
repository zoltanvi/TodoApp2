using Modules.Common.DataBinding;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using Modules.Common.Views.Pages;
using Modules.Settings.Contracts.Models;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

public class SettingsPageViewModel : BaseViewModel
{
    private IServiceProvider _serviceProvider;
    
    public SettingsPageViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        GoBackCommand = new RelayCommand(() =>
        {
            Mediator.NotifyClients(ViewModelMessages.UpdateMainPage);
        });

        SwitchToPageCommand = new RelayParameterizedCommand<object>(SwitchToPage);
        
        SettingsPageFrameType = SettingsPageType.AppWindowSettings;
        SettingsPageFrameContent = GetSettingsPage(SettingsPageFrameType);
    }

    public BasePage SettingsPageFrameContent { get; set; }
    public SettingsPageType SettingsPageFrameType { get; set; }
    public ICommand GoBackCommand { get; }
    public ICommand SwitchToPageCommand { get; }

    private void SwitchToPage(object obj)
    {
        if (obj is SettingsPageType type)
        {
            SettingsPageFrameContent = GetSettingsPage(type);
            SettingsPageFrameType = type;

            OnPropertyChanged(nameof(SettingsPageFrameType));
            OnPropertyChanged(nameof(SettingsPageFrameContent));
        }
    }

    private BasePage GetSettingsPage(SettingsPageType type)
        => type switch
        {
            SettingsPageType.NotePageSettings => GetService<NotePageSettingsPage>(),
            SettingsPageType.TaskItemSettings => GetService<TaskItemSettingsPage>(),
            SettingsPageType.TaskPageSettings => GetService<TaskPageSettingsPage>(),
            SettingsPageType.TaskQuickActionsSettings => GetService<TaskQuickActionsSettingsPage>(),
            SettingsPageType.TextEditorQuickActionsSettings => GetService<TextEditorQuickActionsSettingsPage>(),
            SettingsPageType.ThemeSettings => GetService<ThemeSettingsPage>(),
            SettingsPageType.AppWindowSettings => GetService<ApplicationSettingsPage>(),
            SettingsPageType.PageTitleSettings => GetService<PageTitleSettingsPage>(),
            SettingsPageType.DateTimeSettings => GetService<DateTimeSettingsPage>(),
            SettingsPageType.Shortcuts => GetService<ShortcutsPage>(),
            _ => throw new NotImplementedException($"{type} type setting page is not implemented!")
        };

    private T GetService<T>()
    {
        return (T?)_serviceProvider.GetService(typeof(T));
    }
}
