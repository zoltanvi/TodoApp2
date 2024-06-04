using Modules.Common;
using Modules.Common.DataModels;

namespace Modules.Settings.Contracts.ViewModels;

public class PageTitleSettings : SettingsBase
{
    public bool Visible { get; set; } = true;
    public string Color { get; set; } = Constants.ColorName.Transparent;
    public FontFamily FontFamily { get; set; } = FontFamily.SegoeUIBold;
    public double FontSize { get; set; } = 28;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
}
