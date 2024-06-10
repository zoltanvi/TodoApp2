using Modules.Common.DataBinding;
using Modules.Common.OBSOLETE.Mediator;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Windows.Input;

namespace TodoApp2.Core;

[AddINotifyPropertyChangedInterface]
public class NotificationPageViewModel : BaseViewModel
{
    private bool _closed;

    /// <summary>
    /// The task to show the notification for.
    /// </summary>
    public TaskViewModel NotificationTask => IoC.OverlayPageService.Task;

    /// <summary>
    /// Closes the notification page
    /// </summary>
    public ICommand CloseNotificationCommand { get; }

    public NotificationPageViewModel()
    {
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

            MediatorOBSOLETE.NotifyClients(ViewModelMessages.NotificationClosed, NotificationTask);
        }
    }
}