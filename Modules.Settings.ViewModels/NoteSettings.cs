using Modules.Common.DataModels;

namespace Modules.Settings.ViewModels;

public class NoteSettings : SettingsBase
{
    public bool WordWrap { get; set; }
    public FontFamily FontFamily { get; set; } = FontFamily.Consolas;
}
