using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    [DebuggerDisplay("[id {Id}] [category {CategoryId}] [isDone {IsDone}] [trashed {Trashed}]")]
    public class TaskListItemViewModel : BaseViewModel, IReorderable
    {
        private string m_ContentRollback = string.Empty;
        private bool m_IsDone;

        private IDatabase Database => IoC.Database;

        public string Content
        {
            get => TextEditorViewModel.DocumentContent;
            set => TextEditorViewModel.DocumentContent = value;
        }

        public bool IsDone
        {
            get => m_IsDone;
            set
            {
                m_IsDone = value;
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
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ColorPickerVisible { get; set; }
        public RichTextEditorViewModel TextEditorViewModel { get; }
        public ICommand SetColorCommand { get; }
        public ICommand SetBorderColorCommand { get; }
        public ICommand SetColorParameterizedCommand { get; }
        public ICommand SetBorderColorParameterizedCommand { get; }
        public ICommand OpenReminderCommand { get; }
        public ICommand EditItemCommand { get; }

        public TaskListItemViewModel()
        {
            SetColorCommand = new RelayCommand(SetColor);
            SetBorderColorCommand = new RelayCommand(SetColor);
            SetColorParameterizedCommand = new RelayParameterizedCommand(SetColorParameterized);
            SetBorderColorParameterizedCommand = new RelayParameterizedCommand(SetBorderColorParameterized);
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
            TextEditorViewModel = new RichTextEditorViewModel(true, false, false, false);
            TextEditorViewModel.EnterAction = UpdateContent;
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
            BorderColor = task.BorderColor;
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
                Content = m_ContentRollback;
            }
            // If nothing changed, do not update 
            else if (Content != m_ContentRollback)
            {
                //Modifications are accepted, update task
                ModificationDate = DateTime.Now.Ticks;
                Database.UpdateTask(this);
            }

            TextEditorViewModel.IsEditMode = false;
            TextEditorViewModel.IsToolbarOpen = false;
            IoC.OneEditorOpenService.DisplayMode(this);
        }

        private void EditItem()
        {
            // Save the content before editing for a possible rollback
            m_ContentRollback = Content;

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

        private void SetBorderColorParameterized(object obj)
        {
            if (obj is string colorString)
            {
                // Combobox will trigger the SetColor command so this value will be persisted!
                BorderColor = colorString;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TaskListItemViewModel other && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}