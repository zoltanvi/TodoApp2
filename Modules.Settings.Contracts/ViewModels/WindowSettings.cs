namespace Modules.Settings.Contracts.ViewModels;

public class WindowSettings : SettingsBase
{
    public int Left { get; set; } = 100;
    public int Top { get; set; } = 100;
    public int Width { get; set; } = 400;
    public int Height { get; set; } = 540;
    public double Scaling { get; set; }
}
