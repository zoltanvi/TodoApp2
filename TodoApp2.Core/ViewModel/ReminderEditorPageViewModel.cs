using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ReminderEditorPageViewModel : BaseViewModel, IDisposable
    {
        private bool m_Closed;

        private ReminderNotificationService NotificationService => IoC.ReminderNotificationService;
        private OverlayPageService OverlayPageService => IoC.OverlayPageService;

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskListItemViewModel ReminderTask { get; }
        public DateTime SelectedDate { get; set; }
        public string SelectedDateString { get; set; }
        public bool IsSelectedDateStringValid { get; set; }
        public DateTime SelectedTime { get; set; }
        public bool IsReminderOn
        {
            get => ReminderTask.IsReminderOn;
            set => ReminderTask.IsReminderOn = value;
        }

        public ICommand CloseReminderCommand { get; }
        public ICommand SetReminderCommand { get; }
        public ICommand ResetReminderCommand { get; }
        public ICommand ChangeIsReminderOn { get; }

        public ReminderEditorPageViewModel()
        {
            SetReminderCommand = new RelayCommand(SetReminder);
            ResetReminderCommand = new RelayCommand(ResetReminder);
            CloseReminderCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeReminder);
            OverlayPageService.SetBackgroundClickedAction(ClosePage);
        }

        public ReminderEditorPageViewModel(TaskListItemViewModel reminderTask) : this()
        {
            if (reminderTask != null)
            {
                ReminderTask = IoC.ClientDatabase.GetTask(reminderTask.Id);

                ResetReminderProperties();

                IsSelectedDateStringValid = true;

                PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        private void SetReminder()
        {
            IsReminderOn = true;

            UpdateTaskReminder();

            NotificationService.SetReminder(ReminderTask);

            ClosePage();
        }

        private void ResetReminder()
        {
            ReminderTask.ReminderDate = 0;
            ResetReminderProperties();
            IsReminderOn = false;
            
            IoC.ClientDatabase.UpdateTask(ReminderTask);

            NotificationService.DeleteReminder(ReminderTask);
        }

        private void ChangeReminder()
        {
            UpdateTaskReminder();
        }

        private void ClosePage()
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

            IoC.ClientDatabase.UpdateTask(ReminderTask);
        }

        private void ResetReminderProperties()
        {
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

        protected override void OnDispose()
        {
            if (!m_Closed)
            {
                PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}