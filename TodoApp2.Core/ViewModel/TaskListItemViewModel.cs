using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    public class TaskListItemViewModel : BaseViewModel, IReorderable
    {
        private ClientDatabase Database => IoC.ClientDatabase;

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
        public long ListOrder { get; set; }
        public bool IsDone { get; set; }
        public long CreationDate { get; set; } = DateTime.Now.Ticks;
        public long ModificationDate { get; set; } = DateTime.Now.Ticks;
        public string Color { get; set; }
        public bool Trashed { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ColorPickerVisible { get; set; }
        public bool IsEditMode { get; set; }
        public string PendingEditContent { get; set; }

        public ICommand ShowColorPickerCommand { get; }
        public ICommand HideColorPickerCommand { get; }
        public ICommand SetColorCommand { get; }
        public ICommand OpenReminderCommand { get; }

        public ICommand EditItemCommand { get; }

        public TaskListItemViewModel()
        {
            ShowColorPickerCommand = new RelayCommand(ShowColorPicker);
            HideColorPickerCommand = new RelayCommand(HideColorPicker);
            SetColorCommand = new RelayParameterizedCommand(SetColor);
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
        }

        public void UpdateContent()
        {
            // If the text is empty or only whitespace, refuse
            // If the text only contains format characters, refuse
            string trimmed = PendingEditContent.Replace("`", string.Empty);
            if (!string.IsNullOrWhiteSpace(PendingEditContent) && !string.IsNullOrWhiteSpace(trimmed))
            {
                // Changes are accepted
                Content = PendingEditContent;

                // Persist changes into database
                Database.UpdateTask(this);
            }

            // Switch back from edit mode
            IsEditMode = false;

            // Request a task list refresh
            // (workaround because the task list item does not get repainted even if the Content changes)
            Mediator.Instance.NotifyClients(ViewModelMessages.RefreshTaskListRequested);
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
            //IoC.Application.ReminderTask = this;

            // Request to open the Reminder page
            Mediator.Instance.NotifyClients(ViewModelMessages.OpenReminderPageRequested, this);
        }

        private void SetColor(object obj)
        {
            if (obj is string colorString)
            {
                Color = colorString;
                Database.UpdateTask(this);
            }
        }

        private void HideColorPicker()
        {
            ColorPickerVisible = false;
        }

        private void ShowColorPicker()
        {
            ColorPickerVisible = true;
        }
    }
}