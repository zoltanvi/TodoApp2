namespace TodoApp2;

internal class BoolNegatedConverter: BaseBoolValueConverter<bool>
{
    protected override bool PositiveValue { get; } = false;

    protected override bool NegativeValue { get; } = true;
}
