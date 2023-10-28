namespace TodoApp2.Persistence.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ListOrder { get; set; }
        public bool Trashed { get; set; }
    }
}
