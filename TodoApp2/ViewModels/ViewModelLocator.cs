using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in XAML files
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; } = new ViewModelLocator();
        
        public static ApplicationViewModel ApplicationViewModel => IoC.Application;
        public static OverlayPageService OverlayPageService => IoC.OverlayPageService;
        public static CategoryListService CategoryListService => IoC.CategoryListService;
        public static TaskListService TaskListService => IoC.TaskListService;

        public static ColorListProvider ColorListProvider { get; } = new ColorListProvider();
        public static EnumValuesListProvider<Theme> ThemeListProvider { get; } = new EnumValuesListProvider<Theme>();
        public static EnumValuesListProvider<Thickness> ThicknessListProvider { get; } = new EnumValuesListProvider<Thickness>();
        public static EnumValuesListProvider<FontSize> FontSizeListProvider { get; } = new EnumValuesListProvider<FontSize>();
        public static EnumValuesListProvider<FontFamily> FontFamilyListProvider { get; } = new EnumValuesListProvider<FontFamily>();

        public static AccentColorProvider AccentColorProvider { get; } = new AccentColorProvider();
    }
}