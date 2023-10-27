using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class TaskReminderPageViewModel : BaseViewModel
    {
        private readonly OverlayPageService _overlayPageService;
        private readonly IDatabase _database;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskViewModel ReminderTask { get; }

        public long ReminderDateTime => ReminderTask?.ReminderDate ?? 0;

        public bool IsReminderOnToggle => ReminderDateTime > 0;

        public bool HasValidReminderTime => ReminderDateTime != 0; 

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

        public TaskReminderPageViewModel(TaskViewModel reminderTask, OverlayPageService overlayPageService, IDatabase database)
        {
            if (reminderTask == null)
            {
                throw new ArgumentNullException(nameof(reminderTask));
            }

            _overlayPageService = overlayPageService;
            _database = database;

            EditReminderCommand = new RelayCommand(EditReminder);
            ClosePageCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeIsOn);
            _overlayPageService.SetBackgroundClickedAction(ClosePage);

            ReminderTask = _database.GetTask(reminderTask.Id);
        }

        private void ChangeIsOn()
        {
            // ReminderTask.IsReminderOn is modified with the toggle button,
            // so we persist the modification
            _database.UpdateTask(ReminderTask);
        }

        private void EditReminder()
        {
            _overlayPageService.OpenPage(ApplicationPage.ReminderEditor, ReminderTask);
        }

        private void ClosePage()
        {
            _overlayPageService.ClosePage();
        }
    }
}