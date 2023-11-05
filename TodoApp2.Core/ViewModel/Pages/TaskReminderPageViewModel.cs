using System;
using System.Windows.Input;
using TodoApp2.Core.Mappings;
using TodoApp2.Persistence;

namespace TodoApp2.Core
{
    public class TaskReminderPageViewModel : BaseViewModel
    {
        private readonly OverlayPageService _overlayPageService;
        private readonly IAppContext _context;

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

        public TaskReminderPageViewModel(TaskViewModel reminderTask, OverlayPageService overlayPageService, IAppContext context)
        {
            if (reminderTask == null)
            {
                throw new ArgumentNullException(nameof(reminderTask));
            }

            _overlayPageService = overlayPageService;
            _context = context;

            EditReminderCommand = new RelayCommand(EditReminder);
            ClosePageCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeIsOn);
            _overlayPageService.SetBackgroundClickedAction(ClosePage);

            ReminderTask = _context.Tasks.First(x => x.Id == reminderTask.Id).Map();
        }

        private void ChangeIsOn()
        {
            // ReminderTask.IsReminderOn is modified with the toggle button,
            // so we persist the modification
            _context.Tasks.UpdateFirst(ReminderTask.Map());
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