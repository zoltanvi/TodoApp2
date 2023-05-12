using System.Windows;

namespace TodoApp2
{
    // Used for task list item color bar
    internal class BoolToCornerRadiusConverter : BaseBoolValueConverter<CornerRadius>
    {
        protected override CornerRadius PositiveValue { get; } = new CornerRadius(3, 0, 0, 3);

        protected override CornerRadius NegativeValue { get; } = new CornerRadius(0);
    }
}
