using TodoApp2.Material;

namespace TodoApp2.Core
{
    public class ThemeSettings : SettingsBase
    {
        public bool DarkMode { get; set; }
        public ThemeStyle ThemeStyle { get; set; }
        public string SeedColor { get; set; } = "#1D64DD";
    }
}
