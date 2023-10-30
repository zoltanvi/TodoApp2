using Task = TodoApp2.Persistence.Models.Task;

namespace TodoApp2.Core.Mappings
{
    internal static class TaskMapping
    {
        public static Task Map(this TaskViewModel vm)
        {
            return new Task
            {
                Id = vm.Id,
                CategoryId = vm.CategoryId,
                Content = vm.Content,
                ListOrder = vm.ListOrder.ToString(GlobalConstants.ListOrderFormat),
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
            return new TaskViewModel
            {
                Id = task.Id,
                CategoryId = task.CategoryId,
                Content = task.Content,
                ListOrder = long.Parse(task.ListOrder),
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
    }
}
