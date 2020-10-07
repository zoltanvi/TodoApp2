namespace TodoApp2.Core
{
    public class ReminderViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public long ReminderDate { get; set; }
        public string Note { get; set; }
    }
}