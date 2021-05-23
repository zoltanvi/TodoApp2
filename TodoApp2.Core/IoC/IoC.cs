using Ninject;

namespace TodoApp2.Core
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
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
            var sessionManager = new SessionManager();
            Kernel.Bind<SessionManager>().ToConstant(sessionManager);
            
            var database = new Database(sessionManager.OnlineMode);
            Kernel.Bind<IDatabase>().ToConstant(database);
            
            var applicationViewModel = new ApplicationViewModel(database, sessionManager);
            Kernel.Bind<ApplicationViewModel>().ToConstant(applicationViewModel);

            var overlayPageService = new OverlayPageService(applicationViewModel, database);
            Kernel.Bind<OverlayPageService>().ToConstant(overlayPageService);
            // This dependency must be set here. Workaround to avoid circular dependencies
            applicationViewModel.OverlayPageService = overlayPageService;

            var taskScheduler = new TaskScheduler();
            Kernel.Bind<TaskScheduler>().ToConstant(taskScheduler);

            var reminderNotificationService = new ReminderNotificationService(database, taskScheduler, overlayPageService);
            Kernel.Bind<ReminderNotificationService>().ToConstant(reminderNotificationService);
            // This dependency must be set here. Workaround to avoid circular dependencies
            overlayPageService.ReminderNotificationService = reminderNotificationService;

            var categoryListService = new CategoryListService(applicationViewModel, database);
            Kernel.Bind<CategoryListService>().ToConstant(categoryListService);

            var taskListService = new TaskListService(database, categoryListService);
            Kernel.Bind<TaskListService>().ToConstant(taskListService);
        }
    }
}