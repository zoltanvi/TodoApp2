using Modules.Common.ViewModel;
using System.Windows.Input;

namespace TodoApp2.Core;

public class TimePickerViewModel : BaseViewModel
{
    private int _hour;
    private int _minute;

    public int Hour
    {
        get => _hour;
        set => _hour = value;
    }

    public int Minute
    {
        get => _minute;
        set => _minute = value;
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
        else
        {
            Hour = 0;
        }
    }
    private void DecreaseHour()
    {
        if (Hour - 1 >= 0)
        {
            Hour--;
        }
        else
        {
            Hour = 23;
        }
    }
    private void IncreaseMinute()
    {
        if (Minute + 1 <= 59)
        {
            Minute++;
        }
        else
        {
            Minute = 0;
        }
    }
    private void DecreaseMinute()
    {
        if (Minute - 1 >= 0)
        {
            Minute--;
        }
        else
        {
            Minute = 59;
        }
    }
}
