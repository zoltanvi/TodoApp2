using System;
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

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => Get<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="ClientDatabase"/>
        /// </summary>
        public static ClientDatabase ClientDatabase => Get<ClientDatabase>();

        /// <summary>
        /// A shortcut to access the <see cref="ReminderTaskScheduler"/>
        /// </summary>
        public static TaskScheduler ReminderTaskScheduler => Get<TaskScheduler>();

        /// <summary>
        /// A shortcut to access the <see cref="ReminderNotificationService"/>
        /// </summary>
        public static ReminderNotificationService ReminderNotificationService => Get<ReminderNotificationService>();
        
        /// <summary>
        /// A shortcut to access the <see cref="OverlayPageService"/>
        /// </summary>
        public static OverlayPageService OverlayPageService => Get<OverlayPageService>();

        /// <summary>
        /// A shortcut to access the <see cref="CategoryListService"/>
        /// </summary>
        public static CategoryListService CategoryListService => Get<CategoryListService>();

        /// <summary>
        /// A shortcut to access the <see cref="TaskListService"/>
        /// </summary>
        public static TaskListService TaskListService => Get<TaskListService>();

        /// <summary>
        /// A shortcut to access the <see cref="ColorListService"/>
        /// </summary>
        public static ColorListService ColorListService => Get<ColorListService>();

        /// <summary>
        /// A shortcut to access the <see cref="ThemeListService"/>
        /// </summary>
        public static ThemeListService ThemeListService => Get<ThemeListService>();

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
            BindSingletonViewModels();
            BindSingletonServices();

            // Force the lazy load to initialize the service
            var loadedNotificationService = ReminderNotificationService;
        }


        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindSingletonViewModels()
        {
            // Bind to a single instance
            Kernel.Bind<ApplicationViewModel>().To<ApplicationViewModel>().InSingletonScope();
        }

        /// <summary>
        /// Binds all singleton services
        /// </summary>
        private static void BindSingletonServices()
        {
            Kernel.Bind<ClientDatabase>().To<ClientDatabase>().InSingletonScope();
            Kernel.Bind<TaskScheduler>().To<TaskScheduler>().InSingletonScope();
            Kernel.Bind<ReminderNotificationService>().To<ReminderNotificationService>().InSingletonScope();
            Kernel.Bind<OverlayPageService>().To<OverlayPageService>().InSingletonScope();
            Kernel.Bind<CategoryListService>().To<CategoryListService>().InSingletonScope();
            Kernel.Bind<TaskListService>().To<TaskListService>().InSingletonScope();
            Kernel.Bind<ColorListService>().To<ColorListService>().InSingletonScope();
            Kernel.Bind<ThemeListService>().To<ThemeListService>().InSingletonScope();
        }
    }
}