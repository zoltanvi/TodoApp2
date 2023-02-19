namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for the application
    /// </summary>
    public static class IoC
    {
        public static IAsyncActionService AsyncActionService { get; set; }
        public static AppViewModel ApplicationViewModel { get; private set; }
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

        /// <summary>
        /// Sets up the essential services and modules
        /// </summary>
        public static void PreSetup()
        {
            AutoRunService = new AutoRunService();
            MessageService = new MessageService();
            Database = new Database();
        }

        /// <summary>
        /// Sets up the necessary services, binds all information required
        /// </summary>
        public static void Setup()
        {
            UIScaler = new UIScaler();
            UndoManager = new UndoManager();
            ApplicationViewModel = new AppViewModel(Database, UIScaler);
            OverlayPageService = new OverlayPageService(ApplicationViewModel, Database);
            
            // This dependency must be set here. Workaround to avoid circular dependencies
            ApplicationViewModel.OverlayPageService = OverlayPageService;

            TaskScheduler2 taskScheduler2 = new TaskScheduler2();
            
            ReminderNotificationService reminderNotificationService = 
                new ReminderNotificationService(Database, taskScheduler2, OverlayPageService);
            
            // This dependency must be set here. Workaround to avoid circular dependencies
            OverlayPageService.ReminderNotificationService = reminderNotificationService;

            CategoryListService = new CategoryListService(ApplicationViewModel, Database);
            NoteListService = new NoteListService(ApplicationViewModel, Database);
            TaskListService = new TaskListService(Database, CategoryListService, ApplicationViewModel);

            OneEditorOpenService = new OneEditorOpenService();
        }
    }
}