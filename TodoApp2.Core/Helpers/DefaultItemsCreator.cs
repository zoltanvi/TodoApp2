using TodoApp2.Persistence;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Helpers
{
    internal static class DefaultItemsCreator
    {
        private static IAppContext _context;

        public static void CreateDefaults(IAppContext context)
        {
            _context = context;

            CreateDefaultCategoryIfNotExists();
        }

        private static void CreateDefaultCategoryIfNotExists()
        {
            if (_context.Categories.IsEmpty)
            {
                _context.Categories.Add(new Category { Name = "Today" });
            }
        }
    }
}
