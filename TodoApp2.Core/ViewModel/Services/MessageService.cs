using System;

namespace TodoApp2.Core
{
    public class MessageService : BaseViewModel
    {
        private const int DefaultDuration = 1500;
        private const int UndoMessageDuration = 1500;

        private Guid _timer;
        private bool _initialized;

        public bool MessageLineVisible { get; private set; }
        public bool UndoButtonVisible { get; private set; }
        public MessageType MessageType { get; private set; } = MessageType.Invalid;
        public string Message { get; private set; }

        public MessageService()
        {
            _timer = TimerService.Instance.CreateTimer(TimerOnTick);
        }

        public void ShowUndo(string message)
        {
            //UndoButtonVisible = true;
            ShowMessage(message, UndoMessageDuration, MessageType.Warning);
        }

        public void HideMessage()
        {
            // Immediately hides the message.
            TimerOnTick(this, EventArgs.Empty);
        }

        public void ShowSuccess(string message)
        {
            ShowMessage(message, DefaultDuration, MessageType.Success);
        }

        public void ShowInfo(string message)
        {
            ShowMessage(message, DefaultDuration, MessageType.Info);
        }

        public void ShowWarning(string message)
        {
            ShowMessage(message, DefaultDuration, MessageType.Warning);
        }

        public void ShowError(string message)
        {
            ShowError(message, DefaultDuration);
        }

        public void ShowError(string message, int duration)
        {
            ShowMessage(message, duration, MessageType.Error);
        }

        private void ShowMessage(string message, int duration, MessageType messageType)
        {
            MessageType = messageType;
            MessageLineVisible = true;
            Message = message;

            Initialize();

            TimerService.Instance.ModifyTimerInterval(_timer, duration);
            TimerService.Instance.StartTimer(_timer);
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            MessageLineVisible = false;
            UndoButtonVisible = false;

            TimerService.Instance.StopTimer(_timer);

            //await Task.Delay(600);

            MessageType = MessageType.Invalid;
        }

        // Lazy initialization
        private void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;

                IoC.ThemeChangeNotifier.AddRecipient(this, nameof(MessageType));
                IoC.ThemeChangeNotifier.AddRecipient(this, nameof(MessageLineVisible));
                IoC.ThemeChangeNotifier.AddRecipient(this, nameof(Message));
            }
        }

    }
}
