using System.Windows.Input;

namespace TodoApp2.Core
{
    public class TimePickerViewModel : BaseViewModel
    {
        private int m_Hour;
        private int m_Minute;

        public int Hour
        {
            get => m_Hour;
            set => m_Hour = value;
        }

        public int Minute
        {
            get => m_Minute;
            set => m_Minute = value;
        }

        public ICommand HourUpCommand { get; set; }
        public ICommand HourDownCommand { get; set; }
        public ICommand MinuteUpCommand { get; set; }
        public ICommand MinuteDownCommand { get; set; }


        public TimePickerViewModel()
        {
            HourUpCommand = new RelayCommand(IncreaseHour);
            HourDownCommand = new RelayCommand(DecreaseHour);
            MinuteUpCommand = new RelayCommand(IncreaseMinute);
            MinuteDownCommand = new RelayCommand(DecreaseMinute);
        }

        private void IncreaseHour()
        {
            if (Hour + 1 <= 23)
            {
                Hour++;
            }
        }
        private void DecreaseHour()
        {
            if (Hour - 1 >= 0)
            {
                Hour--;
            }
        }
        private void IncreaseMinute()
        {
            if (Minute + 1 <= 59)
            {
                Minute++;
            }
        }
        private void DecreaseMinute()
        {
            if (Minute - 1 >= 0)
            {
                Minute--;
            }
        }
    }
}
