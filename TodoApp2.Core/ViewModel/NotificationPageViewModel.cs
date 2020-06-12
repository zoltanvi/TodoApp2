using System.Windows.Input;

namespace TodoApp2.Core
{
    public class NotificationPageViewModel : BaseViewModel
    {
        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel NotificationTask { get; set; }

        /// <summary>
        /// The category of the notification task.
        /// </summary>
        public string NotificationTaskCategory
        {
            get
            {
                if (NotificationTask != null)
                {
                    var cat = IoC.ClientDatabase.GetCategory(NotificationTask.CategoryId);
                    return cat.Name;
                }

                return "-- Category not found --";
            }
        }

        /// <summary>
        /// Closes the notification page
        /// </summary>
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
