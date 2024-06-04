namespace Modules.Settings.Contracts.ViewModels;

public class TextEditorQuickActionSettings : SettingsBase
{
    public bool Bold { get; set; } = true;
    public bool Italic { get; set; } = true;
    public bool Underlined { get; set; } = true;
    public bool Small { get; set; }
    public bool Medium { get; set; }
    public bool Large { get; set; }
    public bool Increase { get; set; } = true;
    public bool Decrease { get; set; } = true;
    public bool Monospace { get; set; } = true;
    public bool Reset { get; set; } = true;
    public bool TextColor { get; set; } = true;
    public bool AlignLeft { get; set; }
    public bool AlignCenter { get; set; }
    public bool AlignRight { get; set; }
    public bool AlignJustify { get; set; }
}
