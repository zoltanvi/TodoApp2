using Modules.Common.DataModels;
using Modules.Common.ViewModel;
using Modules.Settings.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2;

public class PropInfo : BaseViewModel
{
    public string Key { get; set; }
    public object Value { get; set; }
}

public class DevSettingsWindowViewModel : BaseViewModel
{
    public ApplicationPage ActiveSettingsPage { get; set; }

    public SettingsBase SelectedSetting { get; set; }

    public ICommand SwitchToPageCommand { get; }

    public ObservableCollection<PropInfo> SelectedSettingPropList { get; set; } = new ObservableCollection<PropInfo>();

    public DevSettingsWindowViewModel()
    {
        SwitchToPageCommand = new RelayParameterizedCommand<object>(SwitchToPage);

        SwitchToPage(ApplicationPage.AppWindowSettings);
    }

    private void SwitchToPage(object obj)
    {
        if (obj is ApplicationPage page)
        {
            ActiveSettingsPage = page;
            SelectedSetting = GetSettingObject(page);

            SelectedSettingPropList.Clear();

            var type = SelectedSetting.GetType();
            
            foreach (var propertyInfo in type.GetProperties())
            {
                var item = new PropInfo()
                {
                    Key = propertyInfo.Name,
                    Value = propertyInfo.GetValue(SelectedSetting)
                };

                SelectedSettingPropList.Add(item);
            }
        }
    }

    private SettingsBase GetSettingObject(ApplicationPage page)
    {
        switch (page)
        {
            case ApplicationPage.NotePageSettings:
            return AppSettings.Instance.NoteSettings;
            case ApplicationPage.TaskItemSettings:
            return AppSettings.Instance.TaskSettings;
            case ApplicationPage.TaskPageSettings:
            return AppSettings.Instance.TaskPageSettings;
            case ApplicationPage.TaskQuickActionsSettings:
            return AppSettings.Instance.TaskQuickActionSettings;
            case ApplicationPage.TextEditorQuickActionsSettings:
            return AppSettings.Instance.TextEditorQuickActionSettings;
            case ApplicationPage.ThemeSettings:
            return AppSettings.Instance.ThemeSettings;
            case ApplicationPage.AppWindowSettings:
            return AppSettings.Instance.AppWindowSettings;
            case ApplicationPage.PageTitleSettings:
            return AppSettings.Instance.PageTitleSettings;
            case ApplicationPage.DateTimeSettings:
            return AppSettings.Instance.DateTimeSettings;
            default:
            throw new ArgumentException("Setting missing in DevSettingsViewModel");
        }
    }
}
