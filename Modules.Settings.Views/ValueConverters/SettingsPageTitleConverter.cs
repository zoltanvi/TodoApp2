using Modules.Common.Views.ValueConverters;
using Modules.Settings.Contracts.Models;
using System.Globalization;

namespace Modules.Settings.Views.ValueConverters;

public class SettingsPageTitleConverter : BaseValueConverter
{
    private Dictionary<SettingsPageType, string> _pageTitles = new Dictionary<SettingsPageType, string>
    {
        { SettingsPageType.AppWindowSettings, "Application settings" },
        { SettingsPageType.ThemeSettings, "Themes" },
        { SettingsPageType.PageTitleSettings, "Page title settings" },
        { SettingsPageType.TaskPageSettings, "Task page settings" },
        { SettingsPageType.TaskItemSettings, "Task item settings" },
        { SettingsPageType.TaskQuickActionsSettings, "Task quick actions" },
        { SettingsPageType.TextEditorQuickActionsSettings, "Text editor quick actions" },
        { SettingsPageType.NotePageSettings, "Note page settings" },
        { SettingsPageType.DateTimeSettings, "Date time settings" },
        { SettingsPageType.Shortcuts, "Shortcuts" },
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SettingsPageType page)
        {
            if (_pageTitles.TryGetValue(page, out var title))
            {
                return title;
            }
        }

        return "TITLE CONVERTER ERROR";
    }
}
