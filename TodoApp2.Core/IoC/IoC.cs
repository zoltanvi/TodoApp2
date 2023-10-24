namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for the application
    /// </summary>
    public static class IoC
    {
        private static ZoomingListener _zoomingListener;
        public static IAsyncActionService AsyncActionService { get; set; }
        public static IResourceUpdater ResourceUpdater { get; set; }
        public static AppSettings AppSettings { get; set; }
        public static AppViewModel AppViewModel { get; private set; }
        public static IDatabase Database { get; private set; }
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

        /// <summary>
        /// Sets up the essential services, modules and the AppSettings
        /// </summary>
        public static void PreSetup()
        {
            AutoRunService = new AutoRunService();
            MessageService = new MessageService();
            Database = new Database();
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

            UndoManager = new UndoManager();
            AppViewModel = new AppViewModel(Database, UIScaler);
            OverlayPageService = new OverlayPageService(AppViewModel, Database);
            
            // Create default categories + help tasks
            Database.AddDefaultRecords();

            // This dependency must be set here. Workaround to avoid circular dependencies
            AppViewModel.OverlayPageService = OverlayPageService;

            TaskScheduler2 taskScheduler2 = new TaskScheduler2();
            
            ReminderNotificationService reminderNotificationService = 
                new ReminderNotificationService(Database, taskScheduler2, OverlayPageService);
            
            // This dependency must be set here. Workaround to avoid circular dependencies
            OverlayPageService.ReminderNotificationService = reminderNotificationService;

            CategoryListService = new CategoryListService(AppViewModel, Database);
            NoteListService = new NoteListService(AppViewModel, Database);
            TaskListService = new TaskListService(Database, CategoryListService, AppViewModel);

            OneEditorOpenService = new OneEditorOpenService();
        }
    }
}