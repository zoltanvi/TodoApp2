using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    [DebuggerDisplay("[id {Id}] [category {CategoryId}] [isDone {IsDone}] [trashed {Trashed}]")]
    public class TaskViewModel : BaseViewModel, IReorderable
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
        public string BackgroundColor { get; set; }
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ColorPickerVisible { get; set; }
        public bool IsQuickActionsEnabled { get; set; }
        public RichTextEditorViewModel TextEditorViewModel { get; }
        public ICommand SetColorCommand { get; }
        public ICommand SetBorderColorCommand { get; }
        public ICommand SetBackgroundColorCommand { get; }
        public ICommand SetColorParameterizedCommand { get; }
        public ICommand SetBorderColorParameterizedCommand { get; }
        public ICommand SetBackgroundColorParameterizedCommand { get; }
        public ICommand OpenReminderCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand EnableQuickActionsCommand { get; }
        public ICommand DisableQuickActionsCommand { get; }

        public TaskViewModel()
        {
            bool focusLostSavesTask = IoC.AppViewModel.ApplicationSettings.FocusLostSavesTask;
            SetColorCommand = new RelayCommand(SetColor);
            SetBorderColorCommand = new RelayCommand(SetColor);
            SetBackgroundColorCommand = new RelayCommand(SetColor);
            SetColorParameterizedCommand = new RelayParameterizedCommand(SetColorParameterized);
            SetBorderColorParameterizedCommand = new RelayParameterizedCommand(SetBorderColorParameterized);
            SetBackgroundColorParameterizedCommand = new RelayParameterizedCommand(SetBackgroundColorParameterized);
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
            TextEditorViewModel = new RichTextEditorViewModel(true, focusLostSavesTask, false, false);
            TextEditorViewModel.EnterAction = UpdateContent;
            EnableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = true);
            DisableQuickActionsCommand = new RelayCommand(() => IsQuickActionsEnabled = false);
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

        private void SetBackgroundColorParameterized(object obj)
        {
            if (obj is string colorString)
            {
                // Combobox will trigger the SetColor command so this value will be persisted!
                BackgroundColor = colorString;
            }
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