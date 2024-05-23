namespace TodoApp2.Core;

public class DateTimeSettings : SettingsBase
{
    public bool TitleBarDateVisible { get; set; }
    public string TitleBarDateFormat { get; set; } = "HH:mm:ss";
    public string ReminderDateFormat { get; set; } = "yyyy MMMM d. dddd, HH:mm:ss";
    public string TaskCreationDateFormat { get; set; } = "yyyy-MM-dd HH:mm";
}

