using System;
using System.Windows.Threading;

namespace TodoApp2.Core
{
    public class MessageService : BaseViewModel
    {
        private const string s_SuccessForeground = "#000000";
        private const string s_InfoForeground = "#FFFFFF";
        private const string s_WarningForeground = "#000000";
        private const string s_ErrorForeground = "#FFFFFF";

        private const string s_SuccessBackground = "#59C359";
        private const string s_InfoBackground = "#1BA1E2";
        private const string s_WarningBackground = "#FFCC00";
        private const string s_ErrorBackground = "#E51400";

        private readonly DispatcherTimer m_Timer;
        private readonly TimeSpan m_MaxInterval;
        private readonly TimeSpan m_DefaultDuration;
        private readonly TimeSpan m_UndoDuration;

        public bool MessageLineVisible { get; private set; }
        public bool UndoButtonVisible { get; private set; }
        public string BackgroundColor { get; private set; } = s_WarningBackground;
        public string TextColor { get; private set; } = s_WarningForeground;
        public string Message { get; private set; }

        public MessageService()
        {
            m_MaxInterval = new TimeSpan(int.MaxValue);
            m_DefaultDuration = TimeSpan.FromSeconds(4);
            m_UndoDuration = TimeSpan.FromSeconds(1);
            m_Timer = new DispatcherTimer{ Interval = m_MaxInterval };
            m_Timer.Tick += TimerOnTick;
        }

        public void ShowUndo(string message)
        {
            //UndoButtonVisible = true;
            ShowMessage(message, m_UndoDuration, s_InfoBackground, s_InfoForeground);
        }

        public void HideMessage()
        {
            // Immediately hides the message.
            TimerOnTick(this, EventArgs.Empty);
        }

        public void ShowSuccess(string message)
        {
            ShowSuccess(message, m_DefaultDuration);
        }

        public void ShowSuccess(string message, TimeSpan duration)
        {
            ShowMessage(message, duration, s_SuccessBackground, s_SuccessForeground);
        }

        public void ShowInfo(string message)
        {
            ShowInfo(message, m_DefaultDuration);
        }

        public void ShowInfo(string message, TimeSpan duration)
        {
            ShowMessage(message, duration, s_InfoBackground, s_InfoForeground);
        }

        public void ShowWarning(string message)
        {
            ShowWarning(message, m_DefaultDuration);
        }

        public void ShowWarning(string message, TimeSpan duration)
        {
            ShowMessage(message, duration, s_WarningBackground, s_WarningForeground);
        }

        public void ShowError(string message)
        {
            ShowError(message, m_DefaultDuration);
        }

        public void ShowError(string message, TimeSpan duration)
        {
            ShowMessage(message, duration, s_ErrorBackground, s_ErrorForeground);
        }

        private void ShowMessage(string message, TimeSpan duration, string backgroundColor, string textColor)
        {
            MessageLineVisible = true;
            Message = message;
            BackgroundColor = backgroundColor;
            TextColor = textColor;

            OnPropertyChanged(nameof(MessageLineVisible));
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(Message));
            
            m_Timer.Interval = duration;
            m_Timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            MessageLineVisible = false;
            UndoButtonVisible = false;
            m_Timer.Stop();
            m_Timer.Interval = m_MaxInterval;
        }
    }
}
