using Modules.Common;
using System;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core.Helpers;

internal static class DefaultItemsCreator
{
    private static IAppContext _context;

    public static void CreateDefaults(IAppContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        CreateDefaultCategoryIfNotExists();
        CreateRecycleBinCategoryIfNotExists();
    }

    private static void CreateDefaultCategoryIfNotExists()
    {
        if (_context.Categories.IsEmpty)
        {
            _context.Categories.AddSimple(new CategoryViewModel
            {
                Name = "Today",
                ListOrder = CommonConstants.DefaultListOrder
            }.Map());

            _context.Tasks.AddSimple(new TaskViewModel
            {
                CategoryId = _context.Categories.First().Id,
                Content = CreateContentXml("This is a sample task."),
                ListOrder = CommonConstants.DefaultListOrder,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks,
                Color = "#00ACC1",
                BorderColor = Constants.ColorName.Transparent,
                BackgroundColor = Constants.ColorName.Transparent
            }.Map());

            _context.Tasks.AddSimple(new TaskViewModel
            {
                CategoryId = _context.Categories.First().Id,
                Content = CreateContentXml("This is a task that has been finished."),
                IsDone = true,
                ListOrder = CommonConstants.DefaultListOrder + CommonConstants.ListOrderInterval,
                CreationDate = DateTime.Now.Ticks,
                ModificationDate = DateTime.Now.Ticks,
                Color = Constants.ColorName.Transparent,
                BorderColor = Constants.ColorName.Transparent,
                BackgroundColor = Constants.ColorName.Transparent
            }.Map());
        }
    }

    private static void CreateRecycleBinCategoryIfNotExists()
    {
        if (_context.Categories.Where(x => x.Id == CommonConstants.RecycleBinCategoryId).Count == 0)
        {
            _context.Categories.AddSimple(new CategoryViewModel
            {
                Id = CommonConstants.RecycleBinCategoryId,
                Name = CommonConstants.RecycleBinCategoryName,
                ListOrder = CommonConstants.MaxListOrder
            }.Map(),
            writeAllProperties: true);
        }
    }

    private static string CreateContentXml(string content)
    {
        return "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            $"<Paragraph><Run>{content}</Run></Paragraph></FlowDocument>";
    }
}
