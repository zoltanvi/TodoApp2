namespace TodoApp2.Core
{
    public class AppSettings : SettingsBase
    {
        public CommonSettings CommonSettings { get; set; } = new CommonSettings();
        public ThemeSettings ThemeSettings { get; set; } = new ThemeSettings();
        public PageTitleSettings PageTitleSettings { get; set; } = new PageTitleSettings();
        public TaskPageSettings TaskPageSettings { get; set; } = new TaskPageSettings();
        public TaskSettings TaskSettings { get; set; } = new TaskSettings();
        public TaskQuickActionSettings TaskQuickActionSettings { get; set; } = new TaskQuickActionSettings();
        public TextEditorQuickActionSettings TextEditorQuickActionSettings { get; set; } = new TextEditorQuickActionSettings();
        public NoteSettings NoteSettings { get; set; } = new NoteSettings();
        public WindowSettings WindowSettings { get; set; } = new WindowSettings();
        public DateTimeSettings DateTimeSettings { get; set; } = new DateTimeSettings();
        public SessionSettings SessionSettings { get; set; } = new SessionSettings();
    }
}
