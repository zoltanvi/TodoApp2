namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for the application
    /// </summary>
    public static class IoC
    {
        public static IAsyncActionService AsyncActionService { get; set; }
        public static ApplicationViewModel ApplicationViewModel { get; private set; }
        public static IDatabase Database { get; private set; }
        public static OverlayPageService OverlayPageService { get; private set; }
        public static CategoryListService CategoryListService { get; private set; }
        public static TaskListService TaskListService { get; private set; }
        public static OneEditorOpenService OneEditorOpenService { get; private set; }
        public static MessageService MessageService { get; private set; }
        public static UIScaler UIScaler { get; private set; }
        public static UndoManager UndoManager { get; private set; }

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all services can be found
        /// </summary>
        public static void Setup()
        {
            UIScaler = new UIScaler();
            UndoManager = new UndoManager();
            MessageService = new MessageService();
            Database = new Database(MessageService);
            ApplicationViewModel = new ApplicationViewModel(Database, UIScaler);
            OverlayPageService = new OverlayPageService(ApplicationViewModel, Database);
            
            // This dependency must be set here. Workaround to avoid circular dependencies
            ApplicationViewModel.OverlayPageService = OverlayPageService;

            TaskScheduler2 taskScheduler2 = new TaskScheduler2();
            
            ReminderNotificationService reminderNotificationService = 
                new ReminderNotificationService(Database, taskScheduler2, OverlayPageService);
            
            // This dependency must be set here. Workaround to avoid circular dependencies
            OverlayPageService.ReminderNotificationService = reminderNotificationService;

            CategoryListService = new CategoryListService(ApplicationViewModel, Database);
            TaskListService = new TaskListService(Database, CategoryListService, ApplicationViewModel);

            OneEditorOpenService = new OneEditorOpenService();
        }
    }
}