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

        public ICommand ShowColorPickerCommand { get; }
        public ICommand HideColorPickerCommand { get; }
        public ICommand SetColorCommand { get; }

        public TaskListItemViewModel()
        {
            ShowColorPickerCommand = new RelayCommand(ShowColorPicker);
            HideColorPickerCommand = new RelayCommand(HideColorPicker);
            SetColorCommand = new RelayParameterizedCommand(SetColor);
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
