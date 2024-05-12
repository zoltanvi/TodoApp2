using TodoApp2.Entity.Model;

namespace TodoApp2.Persistence.Models
{
    public class Task : EntityModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }
        public string ListOrder { get; set; }
        public bool Pinned { get; set; }
        public bool IsDone { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public string Color { get; set; }
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public bool Trashed { get; set; }
        public long TrashedDate { get; set; }
        public long ReminderDate { get; set; }
        public bool IsReminderOn { get; set; }
    }
}
