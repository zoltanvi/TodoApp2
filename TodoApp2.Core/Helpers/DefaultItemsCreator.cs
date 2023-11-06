using System;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;
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
                _context.Categories.Add(new CategoryViewModel
                {
                    Name = "Today",
                    ListOrder = CommonConstants.DefaultListOrder
                }.Map());

                _context.Tasks.Add(new TaskViewModel
                {
                    CategoryId = _context.Categories.First().Id,
                    Content = CreateContentXml("This is a sample task."),
                    ListOrder = CommonConstants.DefaultListOrder,
                    CreationDate = DateTime.Now.Ticks,
                    ModificationDate = DateTime.Now.Ticks,
                    Color = "#00ACC1",
                    BorderColor = CoreConstants.ColorName.Transparent,
                    BackgroundColor = CoreConstants.ColorName.Transparent
                }.Map());

                _context.Tasks.Add(new TaskViewModel
                {
                    CategoryId = _context.Categories.First().Id,
                    Content = CreateContentXml("This is a task that has been finished."),
                    IsDone = true,
                    ListOrder = CommonConstants.DefaultListOrder + CommonConstants.ListOrderInterval,
                    CreationDate = DateTime.Now.Ticks,
                    ModificationDate = DateTime.Now.Ticks,
                    Color = CoreConstants.ColorName.Transparent,
                    BorderColor = CoreConstants.ColorName.Transparent,
                    BackgroundColor = CoreConstants.ColorName.Transparent
                }.Map());

                _context.Notes.Add(new NoteViewModel
                {
                    Title = "Empty note",
                    ListOrder = CommonConstants.DefaultListOrder,
                    CreationDate = DateTime.Now.Ticks,
                    ModificationDate = DateTime.Now.Ticks
                }.Map());
            }
        }

        private static string CreateContentXml(string content)
        {
            return "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                $"<Paragraph><Run>{content}</Run></Paragraph></FlowDocument>";
        }
    }
}
