namespace TodoApp2.Core
{
    public static class PropertyValueHandlers
    {
        public static IPropertyValueHandler Bool { get; } = new BoolPropertyValueHandler();
        public static IPropertyValueHandler Integer { get; } = new IntegerPropertyValueHandler();
        public static IPropertyValueHandler Long { get; } = new LongPropertyValueHandler();
        public static IPropertyValueHandler Double { get; } = new DoublePropertyValueHandler();
        public static IPropertyValueHandler String { get; } = new StringPropertyValueHandler();
        public static IPropertyValueHandler Theme { get; } = new EnumPropertyValueHandler<Theme>();
        public static IPropertyValueHandler Thickness { get; } = new EnumPropertyValueHandler<Thickness>();
        public static IPropertyValueHandler FontFamily { get; } = new EnumPropertyValueHandler<FontFamily>();
        public static IPropertyValueHandler HorizontalAlignment { get; } = new EnumPropertyValueHandler<HorizontalAlignment>();
        public static IPropertyValueHandler TaskSpacing { get; } = new EnumPropertyValueHandler<TaskSpacing>();
        public static IPropertyValueHandler ApplicationPage { get; } = new EnumPropertyValueHandler<ApplicationPage>();
    }
}