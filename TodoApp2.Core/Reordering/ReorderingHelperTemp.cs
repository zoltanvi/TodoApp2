using System.Collections.Generic;
using System.Linq;
using TodoApp2.Common;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Core.Reordering
{
    // TEMP CLASS, Ordering should be refactored
    internal static class ReorderingHelperTemp
    {
        private static Reorderer _reorderer;

        static ReorderingHelperTemp()
        {
            _reorderer = new Reorderer();
        }

        public static void ReorderTask(IAppContext context, TaskViewModel task, int newPosition)
        {
            // Get all non-trashed task items from the task's category
            var activeTasks = context.Tasks
                .Where(x => x.CategoryId == task.CategoryId && !x.Trashed)
                .OrderBy(x => x.ListOrder)
                .Map();

            // Filter out the reordered task
            List<IReorderable> filteredOrderedTasks =
                activeTasks.Where(t => t.Id != task.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = task as IReorderable;

            _reorderer.ReorderItem(filteredOrderedTasks, itemToReorder, newPosition, UpdateListOrder);

            context.Tasks.UpdateFirst(task.Map());
            //TaskChanged?.Invoke(this, new TaskChangedEventArgs(task));

            void UpdateListOrder(IEnumerable<IReorderable> taskList) =>
                context.Tasks.UpdateRange(taskList.Cast<TaskViewModel>().Map(), x => x.Id);
        }

        public static void ReorderCategory(IAppContext context, CategoryViewModel category, int newPosition)
        {
            var activeCategories = context.Categories
                .Where(x => !x.Trashed)
                .OrderBy(x => x.ListOrder)
                .Map();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedCategories =
                activeCategories.Where(c => c.Id != category.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = category as IReorderable;

            _reorderer.ReorderItem(
                orderedCategories,
                itemToReorder,
                newPosition,
            UpdateListOrder);

            context.Categories.UpdateFirst(category.Map());

            void UpdateListOrder(IEnumerable<IReorderable> categoryList) =>
              context.Categories.UpdateRange(categoryList.Cast<CategoryViewModel>().Map(), x => x.Id);
        }

        public static void ReorderNote(IAppContext context, NoteViewModel note, int newPosition)
        {
            var activeNotes = context.Notes
                .Where(x => !x.Trashed)
                .OrderBy(x => x.ListOrder)
                .Map();

            // Get all categories except the reordered one that are not trashed
            List<IReorderable> orderedNotes =
                activeNotes.Where(c => c.Id != note.Id).Cast<IReorderable>().ToList();

            IReorderable itemToReorder = note as IReorderable;

            _reorderer.ReorderItem(
                orderedNotes,
                itemToReorder,
                newPosition,
                UpdateListOrder);

            context.Notes.UpdateFirst(note.Map());

            void UpdateListOrder(IEnumerable<IReorderable> noteList) =>
              context.Notes.UpdateRange(noteList.Cast<NoteViewModel>().Map(), x => x.Id);
        }


        public static long GetTaskListOrder(IAppContext context, bool first)
        {
            var activeItems = context.Tasks.Where(x => !x.Trashed);

            if (activeItems.Count == 0) return CommonConstants.DefaultListOrder;

            if (first)
            {
                return activeItems.OrderBy(x => x.ListOrder).First().Map().ListOrder;
            }
            else
            {
                return activeItems.OrderByDescending(x => x.ListOrder).First().Map().ListOrder;
            }
        }
    }
}
