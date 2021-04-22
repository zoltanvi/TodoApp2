using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        private bool m_Closed;
        private readonly OverlayPageService m_OverlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel NotificationTask { get; }

        /// <summary>
        /// Closes the notification page
        /// </summary>
        public ICommand CloseNotificationCommand { get; }

        public NotificationPageViewModel()
        {
        }

        public NotificationPageViewModel(TaskListItemViewModel notificationTask, OverlayPageService overlayPageService)
        {
            NotificationTask = notificationTask;
            m_OverlayPageService = overlayPageService;

            CloseNotificationCommand = new RelayCommand(CloseNotification);
            m_OverlayPageService.SetBackgroundClickedAction(CloseNotification);
        }

        private void CloseNotification()
        {
            if (!m_Closed)
            {
                m_Closed = true;

                m_OverlayPageService.ClosePage();

                Mediator.NotifyClients(ViewModelMessages.NotificationClosed, NotificationTask);
            }
        }
    }
}