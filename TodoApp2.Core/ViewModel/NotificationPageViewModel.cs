using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        private bool m_Closed;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel NotificationTask { get; set; }

        /// <summary>
        /// Closes the notification page
        /// </summary>
        public ICommand CloseNotificationCommand { get; }

        public NotificationPageViewModel()
        {
            CloseNotificationCommand = new RelayCommand(CloseNotification);
            OverlayPageService.SetBackgroundClickedAction(CloseNotification);
        }

        public NotificationPageViewModel(TaskListItemViewModel notificationTask) : this()
        {
            NotificationTask = notificationTask;
        }

        private void CloseNotification()
        {
            if (!m_Closed)
            {
                m_Closed = true;

                OverlayPageService.ClosePage();

                Mediator.Instance.NotifyClients(ViewModelMessages.NotificationClosed, NotificationTask);
            }
        }
    }
}