using System;
using System.Diagnostics;
using System.Windows.Input;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    [DebuggerDisplay("[id {Id}] [category {CategoryId}] [isDone {IsDone}] [trashed {Trashed}]")]
    public class TaskViewModel : BaseViewModel, IReorderable
    {
        private string _contentRollback = string.Empty;
        private bool _isDone;

        private IAppContext Context => IoC.Context;

        public string Content
        {
            get => TextEditorViewModel.DocumentContent;
            set => TextEditorViewModel.DocumentContent = value;
        }

        public bool IsDone
        {
            get => _isDone;
            set
            {
                _isDone = value;
                TextEditorViewModel.TextOpacity = IsDone ? 0.25 : 1.0;
            }
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public bool Pinned { get; set; }
        public long ListOrder { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public string Color { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ColorPickerVisible { get; set; }
        public bool IsQuickActionsEnabled { get; set; }
        public RichTextEditorViewModel TextEditorViewModel { get; }
        public ICommand OpenReminderCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand EnableQuickActionsCommand { get; }
        public ICommand DisableQuickActionsCommand { get; }
        public INotifiableObject ColorChangedNotification { get; }

        public TaskViewModel()
        {
            bool exitEditOnFocusLost = IoC.AppSettings.TaskPageSettings.ExitEditOnFocusLost;
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
            TextEditorViewModel = new RichTextEditorViewModel(true, exitEditOnFocusLost, false, true);
            TextEditorViewModel.EnterAction = UpdateContent;
            EnableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = true);
            DisableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = false);

            ColorChangedNotification = new NotifiableObject(SetColor);
        }

        public void CopyProperties(TaskViewModel task)
        {
            Id = task.Id;
            CategoryId = task.CategoryId;
            Content = task.Content;
            ListOrder = task.ListOrder;
            IsDone = task.IsDone;
            CreationDate = task.CreationDate;
            ModificationDate = task.ModificationDate;
            Color = task.Color;
            BorderColor = task.BorderColor;
            BackgroundColor = task.BackgroundColor;
            Trashed = task.Trashed;
            ReminderDate = task.ReminderDate;
            IsReminderOn = task.IsReminderOn;
            ColorPickerVisible = task.ColorPickerVisible;
            TextEditorViewModel.IsEditMode = task.TextEditorViewModel.IsEditMode;
        }

        public void UpdateContent()
        {
            if (TextEditorViewModel.IsContentEmpty)
            {
                // Empty content is rejected, roll back the previous content.
                Content = _contentRollback;
            }
            // If nothing changed, do not update 
            else if (Content != _contentRollback)
            {
                //Modifications are accepted, update task
                ModificationDate = DateTime.Now.Ticks;
                Context.Tasks.UpdateFirst(this.Map());
            }

            TextEditorViewModel.IsEditMode = false;
            TextEditorViewModel.IsToolbarOpen = false;
            IoC.OneEditorOpenService.DisplayMode(this);
        }

        private void EditItem()
        {
            // Save the content before editing for a possible rollback
            _contentRollback = Content;

            // Enable editing
            TextEditorViewModel.IsEditMode = true;
            IoC.OneEditorOpenService.EditMode(this);
        }

        private void OpenReminder()
        {
            IoC.OverlayPageService.OpenPage(ApplicationPage.TaskReminder, this);
        }

        private void SetColor()
        {
            // Combobox changes the Color and BorderColor properties directly, we just need to persist it
            Context.Tasks.UpdateFirst(this.Map());
        }

        public override bool Equals(object obj)
        {
            return obj is TaskViewModel other && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}