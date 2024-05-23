using System.Windows.Controls;

namespace TodoApp2;

internal class BoolToScrollBarVisibilityConverter : BaseBoolValueConverter<ScrollBarVisibility>
{
    // Wrap
    protected override ScrollBarVisibility PositiveValue => ScrollBarVisibility.Disabled;

    // No wrap
    protected override ScrollBarVisibility NegativeValue => ScrollBarVisibility.Hidden;
}
