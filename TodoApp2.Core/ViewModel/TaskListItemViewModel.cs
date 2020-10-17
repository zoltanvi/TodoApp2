using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// A view model for each task list item on the task page
    /// </summary>
    [DebuggerDisplay("[id {Id}] [ord {ListOrder}] [cat {CategoryId}] [fin {IsDone}] [del {Trashed}] [txt {Content}]")]
    public class TaskListItemViewModel : BaseViewModel, IReorderable, IComparable
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
        public ICommand UpdateItemContentCommand { get; }

        public TaskListItemViewModel()
        {
            ShowColorPickerCommand = new RelayCommand(ShowColorPicker);
            HideColorPickerCommand = new RelayCommand(HideColorPicker);
            SetColorCommand = new RelayParameterizedCommand(SetColor);
            OpenReminderCommand = new RelayCommand(OpenReminder);
            EditItemCommand = new RelayCommand(EditItem);
            UpdateItemContentCommand = new RelayCommand(UpdateContent);
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
            IoC.OverlayPageService.OpenPage(ApplicationPage.Reminder, this);
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

        public override bool Equals(object obj)
        {
            bool returnValue = false;

            if (obj is TaskListItemViewModel task)
            {
                returnValue = task.Id == Id;
            }

            return returnValue;
        }

        public override int GetHashCode()
        {
            int hashCode = -2010238430;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + CategoryId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Content);
            hashCode = hashCode * -1521134295 + ListOrder.GetHashCode();
            hashCode = hashCode * -1521134295 + IsDone.GetHashCode();
            hashCode = hashCode * -1521134295 + CreationDate.GetHashCode();
            return hashCode;
        }

        int IComparable.CompareTo(object obj)
        {
            return Id.CompareTo(((TaskListItemViewModel)obj).Id);
        }
    }
}