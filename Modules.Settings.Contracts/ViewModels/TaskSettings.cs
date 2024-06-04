using Modules.Common.DataModels;

namespace Modules.Settings.Contracts.ViewModels;

public class TaskSettings : SettingsBase
{
    public TaskSpacing Spacing { get; set; } = TaskSpacing.Normal;
    public bool BorderVisible { get; set; } = true;
    public bool BackgroundVisible { get; set; } = true;
    public bool CreationDateVisible { get; set; }
    public bool ModificationDateVisible { get; set; }
    public bool CircularCheckbox { get; set; } = true;
    public Thickness ColorBarThickness { get; set; } = Thickness.Thick;
    public FontFamily FontFamily { get; set; } = FontFamily.SegoeUI;
    public double FontSize { get; set; } = 16;
}
