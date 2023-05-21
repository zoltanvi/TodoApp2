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
            m_OverlayPageService = overlayPageService;

            CloseNotificationCommand = new RelayCommand(CloseNotification);
            // Commented out: The user cannot accidentaly close the notification
            //m_OverlayPageService.SetBackgroundClickedAction(CloseNotification);
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