namespace Modules.Settings.Contracts.ViewModels;

public class TaskPageSettings : SettingsBase
{
    public bool InsertOrderReversed { get; set; }
    public bool ForceTaskOrderByState { get; set; }
    public bool ExitEditOnFocusLost { get; set; }
    public bool SaveOnEnter { get; set; } = true;
    public bool PlaySoundOnTaskIsDoneChange { get; set; } = true;
    public bool TaskListMargin { get; set; } = true;
    public bool FormattedPasteEnabled { get; set; } = true;
    public bool ProgressBar { get; set; }
    public bool NumberOnProgressBar { get; set; } = true;
    public bool SendButtonVisible { get; set; } = true;
}
