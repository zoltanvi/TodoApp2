namespace TodoApp2;

internal class TaskBorderBrushConverter : DefaultBorderBrushConverter
{
    protected override string DefaultResourceName { get; } = "OutlineVariant";
}
