using System.Collections.Generic;
using System.Linq;
using TodoApp2.Common;
using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Core.Mappings
{
    internal static class TaskMapping
    {
        public static Task Map(this TaskViewModel vm)
        {
            if (vm == null) return null;

            return new Task
            {
                Id = vm.Id,
                CategoryId = vm.CategoryId,
                Content = vm.Content,
                ListOrder = vm.ListOrder.FormatListOrder(),
                Pinned = vm.Pinned,
                IsDone = vm.IsDone,
                CreationDate = vm.CreationDate,
                ModificationDate = vm.ModificationDate,
                Color = vm.Color,
                BorderColor = vm.BorderColor,
                BackgroundColor = vm.BackgroundColor,
                Trashed = vm.Trashed,
                ReminderDate = vm.ReminderDate,
                IsReminderOn = vm.IsReminderOn
            };
        }

        public static TaskViewModel Map(this Task task)
        {
            if (task == null) return null;

            return new TaskViewModel
            {
                Id = task.Id,
                CategoryId = task.CategoryId,
                Content = task.Content,
                ListOrder = ListOrderParsingHelper.ParseListOrder(task.ListOrder),
                Pinned = task.Pinned,
                IsDone = task.IsDone,
                CreationDate = task.CreationDate,
                ModificationDate = task.ModificationDate,
                Color = task.Color,
                BorderColor = task.BorderColor,
                BackgroundColor = task.BackgroundColor,
                Trashed = task.Trashed,
                ReminderDate = task.ReminderDate,
                IsReminderOn = task.IsReminderOn
            };
        }
        public static List<Task> MapList(this IEnumerable<TaskViewModel> vmList) =>
            vmList.Select(x => x.Map()).ToList();

        public static List<TaskViewModel> MapList(this IEnumerable<Task> modelList) =>
            modelList.Select(x => x.Map()).ToList();
    }
}
