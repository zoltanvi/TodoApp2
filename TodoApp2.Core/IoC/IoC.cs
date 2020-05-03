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
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => IoC.Get<ApplicationViewModel>();

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
            // Bind all required view models
            BindViewModels();
        }

        /// <summary>
        /// Binds all singleton view model
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of Application view model
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());

            // Bind to a single instance of ClientDataBase 
            Kernel.Bind<ClientDatabase>().To<ClientDatabase>().InSingletonScope();
        }
    }
}
