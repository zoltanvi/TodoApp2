using TodoApp2.Entity.Model;

namespace TodoApp2.Persistence.Models
{
    public class Note : EntityModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ListOrder { get; set; }
        public string Content { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public bool Trashed { get; set; }
    }
}
