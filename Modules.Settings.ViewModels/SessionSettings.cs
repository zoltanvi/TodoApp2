using Modules.Common.DataModels;

namespace Modules.Settings.ViewModels;

public class SessionSettings : SettingsBase
{
    public ApplicationPage MainPage { get; set; }
    public ApplicationPage SideMenuPage { get; set; }
    public double SideMenuWidth { get; set; } // 0 = closed by default
    public bool SideMenuOpen { get; set; }
    public int ActiveCategoryId { get; set; } = 1;
    public int ActiveNoteId { get; set; }
}
