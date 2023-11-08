using System.Windows.Input;
using TodoApp2.Common;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        private bool _closed;

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

        public NotificationPageViewModel(TaskViewModel notificationTask)
        {
            ThrowHelper.ThrowIfNull(notificationTask);

            NotificationTask = notificationTask;

            CloseNotificationCommand = new RelayCommand(CloseNotification);
            
            // Commented out: The user cannot accidentaly close the notification
            //_overlayPageService.SetBackgroundClickedAction(CloseNotification);
        }

        private void CloseNotification()
        {
            if (!_closed)
            {
                _closed = true;

                IoC.OverlayPageService.ClosePage();

                Mediator.NotifyClients(ViewModelMessages.NotificationClosed, NotificationTask);
            }
        }
    }
}