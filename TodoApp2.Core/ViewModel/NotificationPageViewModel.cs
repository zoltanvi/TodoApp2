using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        public ICommand CloseNotificationCommand { get; }

        public NotificationPageViewModel()
        {
            CloseNotificationCommand = new RelayCommand(CloseNotification);
        }

        private void CloseNotification()
        {
            Mediator.Instance.NotifyClients(ViewModelMessages.CloseNotificationPageRequested);
        }
    }
}
