using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class MessageService : BaseViewModel
    {
        private readonly DispatcherTimer m_Timer;
        private readonly TimeSpan m_MaxInterval;
        private readonly TimeSpan m_DefaultDuration;
        private readonly TimeSpan m_UndoDuration;

        public bool MessageLineVisible { get; private set; }
        public bool UndoButtonVisible { get; private set; }
        public MessageType MessageType { get; private set; } = MessageType.Invalid;
        public string Message { get; private set; }

        public MessageService()
        {
            m_MaxInterval = new TimeSpan(int.MaxValue);
            m_DefaultDuration = TimeSpan.FromSeconds(1.5);
            m_UndoDuration = TimeSpan.FromSeconds(1.5);
            m_Timer = new DispatcherTimer { Interval = m_MaxInterval };
            m_Timer.Tick += TimerOnTick;
        }

        public void ShowUndo(string message)
        {
            //UndoButtonVisible = true;
            ShowMessage(message, m_UndoDuration, MessageType.Warning);
        }

        public void HideMessage()
        {
            // Immediately hides the message.
            TimerOnTick(this, EventArgs.Empty);
        }

        public void ShowSuccess(string message)
        {
            ShowMessage(message, m_DefaultDuration, MessageType.Success);
        }

        public void ShowInfo(string message)
        {
            ShowMessage(message, m_DefaultDuration, MessageType.Info);
        }

        public void ShowWarning(string message)
        {
            ShowMessage(message, m_DefaultDuration, MessageType.Warning);
        }

        public void ShowError(string message)
        {
            ShowError(message, m_DefaultDuration);
        }

        public void ShowError(string message, TimeSpan duration)
        {
            ShowMessage(message, duration, MessageType.Error);
        }

        private void ShowMessage(string message, TimeSpan duration, MessageType messageType)
        {
            MessageType = messageType;
            MessageLineVisible = true;

            Message = message;

            OnPropertyChanged(nameof(MessageType));
            OnPropertyChanged(nameof(MessageLineVisible));
            OnPropertyChanged(nameof(Message));

            m_Timer.Interval = duration;
            m_Timer.Start();
        }

        private async void TimerOnTick(object sender, EventArgs e)
        {
            MessageLineVisible = false;
            UndoButtonVisible = false;

            m_Timer.Stop();
            m_Timer.Interval = m_MaxInterval;

            //await Task.Delay(600);

            MessageType = MessageType.Invalid;
        }
    }
}
