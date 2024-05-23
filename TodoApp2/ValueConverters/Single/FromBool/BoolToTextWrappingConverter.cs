using System.Windows;

namespace TodoApp2;

public class BoolToTextWrappingConverter : BaseBoolValueConverter<TextWrapping>
{
    protected override TextWrapping PositiveValue => TextWrapping.Wrap;
    protected override TextWrapping NegativeValue => TextWrapping.NoWrap;
}
