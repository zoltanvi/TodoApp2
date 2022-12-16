using Ninject;

namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        public static IAsyncActionService AsyncActionService { get; set; }
        
        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        private static IKernel Kernel { get; } = new StandardKernel();

        public static ApplicationViewModel ApplicationViewModel => Get<ApplicationViewModel>();

        public static IDatabase Database => Get<IDatabase>();

        public static OverlayPageService OverlayPageService => Get<OverlayPageService>();

        public static CategoryListService CategoryListService => Get<CategoryListService>();

        public static TaskListService TaskListService => Get<TaskListService>();

        public static SessionManager SessionManager => Get<SessionManager>();

        public static MessageService MessageService => Get<MessageService>();

        public static UIScaler UIScaler => Get<UIScaler>();

        public static UndoManager UndoManager => Get<UndoManager>();

        public static TextDeserializer TextDeserializer => Get<TextDeserializer>();

        /// <summary>
        /// Gets a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all
        ///       services can be found
        /// </summary>
        public static void Setup()
        {
            var textDeserializer = new TextDeserializer();
            Kernel.Bind<TextDeserializer>().ToConstant(textDeserializer);

            var uiScaler = new UIScaler();
            Kernel.Bind<UIScaler>().ToConstant(uiScaler);

            var undoManager = new UndoManager();
            Kernel.Bind<UndoManager>().ToConstant(undoManager);

            var messageService = new MessageService();
            Kernel.Bind<MessageService>().ToConstant(messageService);

            // TODO: Disabled feature
            //var sessionManager = new SessionManager(messageService);
            //Kernel.Bind<SessionManager>().ToConstant(sessionManager);
            SessionManager sessionManager = null;

            var database = new Database(sessionManager, messageService);
            Kernel.Bind<IDatabase>().ToConstant(database);

            var applicationViewModel = new ApplicationViewModel(database, sessionManager, uiScaler);
            Kernel.Bind<ApplicationViewModel>().ToConstant(applicationViewModel);

            var overlayPageService = new OverlayPageService(applicationViewModel, database);
            Kernel.Bind<OverlayPageService>().ToConstant(overlayPageService);
            // This dependency must be set here. Workaround to avoid circular dependencies
            applicationViewModel.OverlayPageService = overlayPageService;

            var taskScheduler2 = new TaskScheduler2();
            Kernel.Bind<TaskScheduler2>().ToConstant(taskScheduler2);

            var reminderNotificationService = new ReminderNotificationService(database, taskScheduler2, overlayPageService);
            Kernel.Bind<ReminderNotificationService>().ToConstant(reminderNotificationService);
            // This dependency must be set here. Workaround to avoid circular dependencies
            overlayPageService.ReminderNotificationService = reminderNotificationService;

            var categoryListService = new CategoryListService(applicationViewModel, database);
            Kernel.Bind<CategoryListService>().ToConstant(categoryListService);

            var taskListService = new TaskListService(database, categoryListService, applicationViewModel);
            Kernel.Bind<TaskListService>().ToConstant(taskListService);
        }
    }
}