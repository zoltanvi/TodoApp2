namespace Modules.Settings.Contracts.ViewModels;

public class TaskQuickActionSettings : SettingsBase
{
    public bool AnyEnabled => Enabled &&
        (ReminderEnabled ||
        ColorEnabled ||
        BackgroundColorEnabled ||
        BorderColorEnabled ||
        PinEnabled ||
        TrashEnabled);

    public bool Enabled { get; set; } = true;
    public bool CheckboxEnabled { get; set; } = true;
    public bool ReminderEnabled { get; set; } = true;
    public bool ColorEnabled { get; set; } = true;
    public bool BackgroundColorEnabled { get; set; }
    public bool BorderColorEnabled { get; set; }
    public bool PinEnabled { get; set; } = true;
    public bool TrashEnabled { get; set; } = true;
}
