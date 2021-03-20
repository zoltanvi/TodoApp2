using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Locates view models from the IoC for use in binding in XAML files
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => IoC.Application;

        /// <summary>
        /// The overlay page service
        /// </summary>
        public static OverlayPageService OverlayPageService => IoC.OverlayPageService;

        /// <summary>
        /// The category list service
        /// </summary>
        public static CategoryListService CategoryListService => IoC.CategoryListService;
    }
}