using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in XAML files
    /// </summary>
    public class VML
    {
        public static VML Instance { get; } = new VML();

        public static AppViewModel ApplicationViewModel => IoC.AppViewModel;
        public static AppSettings AppSettings => IoC.AppSettings;
        public static OverlayPageService OverlayPageService => IoC.OverlayPageService;
        public static CategoryListService CategoryListService => IoC.CategoryListService;
        public static NoteListService NoteListService => IoC.NoteListService;
        public static TaskListService TaskListService => IoC.TaskListService;
        public static ThemeListService ThemeListService { get; } = new ThemeListService();
        public static MessageService MessageService => IoC.MessageService;
        public static UIScaler UIScaler => IoC.UIScaler;

        public static ColorListProvider ColorListProvider { get; } = new ColorListProvider();

        public static AccentColorProvider AccentColorProvider { get; } = new AccentColorProvider();
        public static CustomDropHandler CustomDropHandler { get; } = new CustomDropHandler();
    }
}