using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class ReminderEditorPageViewModel : BaseViewModel
    {
        private bool _closed;

        private readonly IDatabase _database;
        private readonly ReminderNotificationService _notificationService;
        private readonly OverlayPageService _overlayPageService;

        public TimePickerViewModel TimePickerViewModel { get; set; }

        /// <summary>
        /// The task to show the notification for.
        /// </summary>
        public TaskViewModel ReminderTask { get; }
        public DateTime SelectedDate { get; set; }
        public string SelectedDateString { get; set; }
        public bool IsSelectedDateStringValid { get; set; }
        public bool IsReminderOn
        {
            get => ReminderTask.IsReminderOn;
            set => ReminderTask.IsReminderOn = value;
        }

        #region Workaround
        // WORKAROUND properties for MultiBinding bug
        // See: https://stackoverflow.com/questions/22536645/what-hardware-platform-difference-could-cause-an-xaml-wpf-multibinding-to-checkb
        public double MyWidth { get; set; }
        public double MyHeight { get; set; }
        public Rect ClipRect => new Rect(0, 0, MyWidth, MyHeight);
        #endregion Workaround

        public ICommand CloseReminderCommand { get; }
        public ICommand SetReminderCommand { get; }
        public ICommand ResetReminderCommand { get; }
        public ICommand ChangeIsReminderOn { get; }

        public ReminderEditorPageViewModel()
        {
        }

        public ReminderEditorPageViewModel(TaskViewModel reminderTask, OverlayPageService overlayPageService,
            IDatabase database, ReminderNotificationService notificationService)
        {
            if (reminderTask == null)
            {
                throw new ArgumentNullException(nameof(reminderTask));
            }

            _database = database;
            _overlayPageService = overlayPageService;
            _notificationService = notificationService;

            SetReminderCommand = new RelayCommand(SetReminder);
            ResetReminderCommand = new RelayCommand(ResetReminder);
            CloseReminderCommand = new RelayCommand(ClosePage);
            ChangeIsReminderOn = new RelayCommand(ChangeReminder);
            _overlayPageService.SetBackgroundClickedAction(ClosePage);

            TimePickerViewModel = new TimePickerViewModel();

            ReminderTask = _database.GetTask(reminderTask.Id);

            ResetReminderProperties();

            IsSelectedDateStringValid = true;

            PropertyChanged += OnViewModelPropertyChanged;
        }

        private void SetReminder()
        {
            IsReminderOn = true;

            UpdateTaskReminder();

            _notificationService.SetReminder(ReminderTask);

            ClosePage();
        }

        private void ResetReminder()
        {
            ReminderTask.ReminderDate = 0;
            ResetReminderProperties();
            IsReminderOn = false;

            _database.UpdateTask(ReminderTask);

            _notificationService.DeleteReminder(ReminderTask);
        }

        private void ChangeReminder()
        {
            UpdateTaskReminder();
        }

        private void ClosePage()
        {
            if (!_closed)
            {
                _closed = true;

                PropertyChanged -= OnViewModelPropertyChanged;

                _overlayPageService.ClosePage();
            }
        }

        private void UpdateTaskReminder()
        {
            DateTime reminderDate = SelectedDate.Date + new TimeSpan(TimePickerViewModel.Hour, TimePickerViewModel.Minute, 0);
            ReminderTask.ReminderDate = reminderDate.Ticks;

            _database.UpdateTask(ReminderTask);
        }

        private void ResetReminderProperties()
        {
            if (ReminderTask.ReminderDate != 0)
            {
                DateTime date = new DateTime(ReminderTask.ReminderDate);
                SelectedDate = date.Date;
                TimePickerViewModel.Hour = date.Hour;
                TimePickerViewModel.Minute = date.Minute;
                SelectedDateString = SelectedDate.ConvertToString();
            }
            else
            {
                DateTime fiveMinutesFromCurrentTime = DateTime.Now.AddMinutes(5);
                SelectedDateString = fiveMinutesFromCurrentTime.ConvertToString();
                SelectedDate = fiveMinutesFromCurrentTime;
                TimePickerViewModel.Hour = fiveMinutesFromCurrentTime.Hour;
                TimePickerViewModel.Minute = fiveMinutesFromCurrentTime.Minute;
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
            if (!_closed)
            {
                PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
    }
}