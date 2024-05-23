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
            return IoC.AppSettings.NoteSettings;
            case ApplicationPage.TaskItemSettings:
            return IoC.AppSettings.TaskSettings;
            case ApplicationPage.TaskPageSettings:
            return IoC.AppSettings.TaskPageSettings;
            case ApplicationPage.TaskQuickActionsSettings:
            return IoC.AppSettings.TaskQuickActionSettings;
            case ApplicationPage.TextEditorQuickActionsSettings:
            return IoC.AppSettings.TextEditorQuickActionSettings;
            case ApplicationPage.ThemeSettings:
            return IoC.AppSettings.ThemeSettings;
            case ApplicationPage.AppWindowSettings:
            return IoC.AppSettings.AppWindowSettings;
            case ApplicationPage.PageTitleSettings:
            return IoC.AppSettings.PageTitleSettings;
            case ApplicationPage.DateTimeSettings:
            return IoC.AppSettings.DateTimeSettings;
            default:
            throw new ArgumentException("Setting missing in DevSettingsViewModel");
        }
    }
}
