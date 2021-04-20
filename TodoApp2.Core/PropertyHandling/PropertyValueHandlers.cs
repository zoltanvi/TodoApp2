﻿namespace TodoApp2.Core
{
    public static class PropertyValueHandlers
    {
        public static IPropertyValueHandler Bool { get; } = new BoolPropertyValueHandler();
        public static IPropertyValueHandler Integer { get; } = new IntegerPropertyValueHandler();
        public static IPropertyValueHandler Long { get; } = new LongPropertyValueHandler();
        public static IPropertyValueHandler String { get; } = new StringPropertyValueHandler();
        public static IPropertyValueHandler Theme { get; } = new ThemePropertyValueHandler();
        public static IPropertyValueHandler Thickness { get; } = new ThicknessPropertyValueHandler();
    }
}