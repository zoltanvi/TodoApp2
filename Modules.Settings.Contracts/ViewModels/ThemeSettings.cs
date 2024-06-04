using Modules.Common.DataModels;

namespace Modules.Settings.Contracts.ViewModels;

public class ThemeSettings : SettingsBase
{
    public bool DarkMode { get; set; } = true;
    public MaterialThemeStyle ThemeStyle { get; set; } = MaterialThemeStyle.TonalSpot;
    public string SeedColor { get; set; } = "#1D64DD";
}
