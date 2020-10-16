using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ReminderPageViewModel : BaseViewModel
    {
        private bool m_Closed = false;

        private ReminderNotificationService NotificationService => IoC.ReminderNotificationService;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel ReminderTask { get; set; }

        public DateTime SelectedDate { get; set; }
        
        public string SelectedDateString { get; set; }

        public bool IsSelectedDateStringValid { get; set; }

        public DateTime SelectedTime { get; set; }

        public bool IsReminderOn { get; set; }

        public ICommand CloseReminderCommand { get; }

        public ICommand SetReminderCommand { get; }

        public ReminderPageViewModel()
        {
            SetReminderCommand = new RelayCommand(SetReminder);
            CloseReminderCommand = new RelayCommand(CloseReminder);
            OverlayPageService.SetBackgroundClickedAction(CloseReminder);
        }

        public ReminderPageViewModel(TaskListItemViewModel reminderTask) : this()
        {
            if (reminderTask != null)
            {
                ReminderTask = reminderTask;
                IsReminderOn = ReminderTask.IsReminderOn;
                if (ReminderTask.ReminderDate != 0)
                {
                    DateTime date = new DateTime(ReminderTask.ReminderDate);
                    SelectedDate = date.Date;
                    SelectedTime = new DateTime() + date.TimeOfDay;
                    SelectedDateString = SelectedDate.ConvertToString();
                }
                else
                {
                    SelectedDateString = DateTime.Now.ConvertToString();
                    SelectedDate = DateTime.Now;
                    SelectedTime = DateTime.Now + new TimeSpan(0, 5, 0);
                }
                IsSelectedDateStringValid = true;

                PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void SetReminder()
        {
            UpdateTaskReminder();

            if (IsReminderOn)
            {
                NotificationService.SetReminder(ReminderTask);
            }
            else
            {
                NotificationService.DeleteReminder(ReminderTask);
            }

            CloseReminder();
        }

        private void CloseReminder()
        {
            if (!m_Closed)
            {
                m_Closed = true;
                
                PropertyChanged -= OnViewModelPropertyChanged;

                OverlayPageService.ClosePage();
            }
        }

        private void UpdateTaskReminder()
        {
            DateTime reminderDate = SelectedDate.Date + new TimeSpan(SelectedTime.Hour, SelectedTime.Minute, 0);
            ReminderTask.ReminderDate = reminderDate.Ticks;

            ReminderTask.IsReminderOn = IsReminderOn;
            IoC.ClientDatabase.UpdateTask(ReminderTask);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedDateString):
                {
                    IsSelectedDateStringValid = false;
                    
                    if (SelectedDateString.ConvertToDate(out DateTime selectedDate))
                    {
                        SelectedDate = selectedDate;
                        IsSelectedDateStringValid = true;
                    }
                    break;
                }
                case nameof(SelectedDate):
                {
                    SelectedDateString = SelectedDate.ConvertToString();
                    IsSelectedDateStringValid = true;
                    break;
                }
            }
        }
    }
}