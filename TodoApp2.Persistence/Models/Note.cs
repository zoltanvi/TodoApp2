namespace TodoApp2.Persistence.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long ListOrder { get; set; }
        public string Content { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public bool Trashed { get; set; }
    }
}
