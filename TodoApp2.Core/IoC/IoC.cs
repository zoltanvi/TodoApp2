using TodoApp2.Core.Helpers;
using TodoApp2.Core.Services;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for the application
    /// </summary>
    public static class IoC
    {
        private static TaskScheduler2 _taskScheduler;
        private static ZoomingListener _zoomingListener;
        public static IAppContext Context { get; private set; }
        public static IAsyncActionService AsyncActionService { get; set; }
        public static ITaskContentSplitterService TaskContentSplitterService { get; set; }
        public static AppSettings AppSettings { get; set; }
        public static AppViewModel AppViewModel { get; private set; }
        public static OverlayPageService OverlayPageService { get; private set; }
        public static CategoryListService CategoryListService { get; private set; }
        public static TaskListService TaskListService { get; private set; }
        public static NoteListService NoteListService { get; private set; }
        public static OneEditorOpenService OneEditorOpenService { get; private set; }
        public static MessageService MessageService { get; private set; }
        public static UIScaler UIScaler { get; private set; }
        public static UndoManager UndoManager { get; private set; }
        public static AutoRunService AutoRunService { get; private set; }
        public static MediaPlayerService MediaPlayerService { get; private set; }
        public static IThemeManagerService ThemeManagerService { get; set; }
        public static ThemeChangeNotifier ThemeChangeNotifier { get; private set; }
        public static ReminderNotificationService ReminderNotificationService { get; private set; }

        /// <summary>
        /// Sets up the essential services, modules and the AppSettings
        /// </summary>
        public static void PreSetup()
        {
            Context = DataAccess.GetAppContext();
            AutoRunService = new AutoRunService();
            MessageService = new MessageService();
            AppSettings = new AppSettings();
        }

        /// <summary>
        /// Sets up the necessary services, binds all information required
        /// </summary>
        public static void Setup()
        {
            MediaPlayerService = new MediaPlayerService();
            UIScaler = UIScaler.Instance;
            
            _zoomingListener = new ZoomingListener(UIScaler, AppSettings);
            ThemeChangeNotifier = new ThemeChangeNotifier();
            ThemeChangeNotifier.AddRecipient(AppSettings.AppWindowSettings, nameof(AppWindowSettings.AppBorderColor));

            UndoManager = new UndoManager();
            AppViewModel = new AppViewModel(Context, UIScaler);
            OverlayPageService = new OverlayPageService(Context, AppViewModel);

            // Create default categories + example tasks
            DefaultItemsCreator.CreateDefaults(Context);
            
            CategoryListService = new CategoryListService(Context, AppViewModel);
            TaskListService = new TaskListService(Context, AppViewModel, CategoryListService);
            NoteListService = new NoteListService(Context, AppViewModel);

            _taskScheduler = new TaskScheduler2();

            ReminderNotificationService = new ReminderNotificationService(Context, _taskScheduler, TaskListService);
            
            OneEditorOpenService = new OneEditorOpenService();
        }
    }
}