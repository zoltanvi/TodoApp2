using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class TaskReminderPageViewModel : BaseViewModel
    {
        private readonly OverlayPageService m_OverlayPageService;
        private readonly IDatabase m_Database;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel ReminderTask { get; }
        public long ReminderDateTime => ReminderTask?.ReminderDate ?? 0;
        public bool IsReminderOn
        {
            get => ReminderTask.IsReminderOn;
            set => ReminderTask.IsReminderOn = value;
        }

        public ICommand ClosePageCommand { get; }
        public ICommand EditReminderCommand { get; }
        public ICommand ChangeIsReminderOn { get; }

        public TaskReminderPageViewModel()
        {
        }

        public TaskReminderPageViewModel(TaskListItemViewModel reminderTask, OverlayPageService overlayPageService, IDatabase database)
        {
            if (reminderTask == null)
            {
                throw new ArgumentNullException(nameof(reminderTask));
            }

            m_OverlayPageService = overlayPageService;
            m_Database = database;

            EditReminderCommand = new RelayCommand(EditReminder);
            ClosePageCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeIsOn);
            m_OverlayPageService.SetBackgroundClickedAction(ClosePage);
            
            ReminderTask = m_Database.GetTask(reminderTask.Id);
        }

        private void ChangeIsOn()
        {
            // ReminderTask.IsReminderOn is modified with the toggle button,
            // so we persist the modification
            m_Database.UpdateTask(ReminderTask);
        }

        private void EditReminder()
        {
            m_OverlayPageService.OpenPage(ApplicationPage.ReminderEditor, ReminderTask);
        }

        private void ClosePage()
        {
            m_OverlayPageService.ClosePage();
        }
    }
}