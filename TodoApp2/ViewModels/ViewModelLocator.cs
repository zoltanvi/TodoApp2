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
        public static ColorListProvider ColorListProvider => IoC.ColorListProvider;
        public static ThemeListProvider ThemeListProvider => IoC.ThemeListProvider;
        public static ThicknessListProvider ThicknessListProvider => IoC.ThicknessListProvider;
    }
}