namespace TodoApp2.Core
{
    public class NoteSettings : SettingsBase
    {
        public bool WordWrap { get; set; }
        public FontFamily FontFamily { get; set; } = FontFamily.Consolas;
    }
}
