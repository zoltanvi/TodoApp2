using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    [DebuggerDisplay("[id {Id}] [category {CategoryId}] [isDone {IsDone}] [trashed {Trashed}] [content {Content}]")]
    public class TaskListItemViewModel : BaseViewModel, IReorderable
    {
        private IDatabase Database => IoC.Database;

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
        public bool Pinned { get; set; }
        public long ListOrder { get; set; }
        public bool IsDone { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public string Color { get; set; }
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ColorPickerVisible { get; set; }
        public bool IsEditMode { get; set; }
        public string PendingEditContent { get; set; }

        public ICommand SetColorCommand { get; }
        public ICommand SetColorParameterizedCommand { get; }
        public ICommand OpenReminderCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand UpdateItemContentCommand { get; }

        public TaskListItemViewModel()
        {
            SetColorCommand = new RelayCommand(SetColor);
            SetColorParameterizedCommand = new RelayParameterizedCommand(SetColorParameterized);
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
            UpdateItemContentCommand = new RelayCommand(UpdateContent);
        }

        public void CopyProperties(TaskListItemViewModel task)
        {
            Id = task.Id;
            CategoryId = task.CategoryId;
            Content = task.Content;
            ListOrder = task.ListOrder;
            IsDone = task.IsDone;
            CreationDate = task.CreationDate;
            ModificationDate = task.ModificationDate;
            Color = task.Color;
            Trashed = task.Trashed;
            ReminderDate = task.ReminderDate;
            IsReminderOn = task.IsReminderOn;
            ColorPickerVisible = task.ColorPickerVisible;
            IsEditMode = task.IsEditMode;
            PendingEditContent = task.PendingEditContent;
        }

        public void UpdateContent()
        {
            // If the text is empty or only whitespace, refuse
            // If the text only contains format characters, refuse
            // If the content did not changed, refuse
            string trimmed = PendingEditContent?.Replace("`", string.Empty);
            if (!string.IsNullOrWhiteSpace(PendingEditContent) &&
                !string.IsNullOrWhiteSpace(trimmed) &&
                Content != PendingEditContent)
            {
                // 1. Changes are accepted
                Content = PendingEditContent;

                // 2. Update modification date
                ModificationDate = DateTime.Now.Ticks;

                // 3. Persist changes into database
                Database.UpdateTask(this);
            }

            // Switch back from edit mode
            IsEditMode = false;

            // Clear edit text
            PendingEditContent = string.Empty;
        }

        private void EditItem()
        {
            // Copy the content as the pending text
            PendingEditContent = Content;

            // Enable editing
            IsEditMode = true;
        }

        private void OpenReminder()
        {
            IoC.OverlayPageService.OpenPage(ApplicationPage.TaskReminder, this);
        }

        private void SetColor()
        {
            // Combobox changes the Color property directly, we just need to persist it
            Database.UpdateTask(this);
        }

        private void SetColorParameterized(object obj)
        {
            if (obj is string colorString)
            {
                // Combobox will trigger the SetColor command so this value will be persisted!
                Color = colorString;
            }
        }
    }
}