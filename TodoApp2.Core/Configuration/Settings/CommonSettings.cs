namespace TodoApp2.Core
{
    public class CommonSettings : SettingsBase
    {
        public bool AlwaysOnTop { get; set; }
        public bool AutoStart { get; set; }
        public bool RoundedWindowCorners { get; set; } = true;
        public bool ExitToTray { get; set; }
        public bool CloseSideMenuOnCategoryChange { get; set; } = true;
        public string AppBorderColor { get; set; } = "#BDBDBD";
    }
}
