using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        private bool _closed;
        private readonly OverlayPageService _overlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskViewModel NotificationTask { get; }

        /// <summary>
        /// Closes the notification page
        /// </summary>
        public ICommand CloseNotificationCommand { get; }

        public NotificationPageViewModel()
        {
        }

        public NotificationPageViewModel(TaskViewModel notificationTask, OverlayPageService overlayPageService)
        {
            NotificationTask = notificationTask;
            _overlayPageService = overlayPageService;

            CloseNotificationCommand = new RelayCommand(CloseNotification);
            // Commented out: The user cannot accidentaly close the notification
            //_overlayPageService.SetBackgroundClickedAction(CloseNotification);
        }

        private void CloseNotification()
        {
            if (!_closed)
            {
                _closed = true;

                _overlayPageService.ClosePage();

                Mediator.NotifyClients(ViewModelMessages.NotificationClosed, NotificationTask);
            }
        }
    }
}