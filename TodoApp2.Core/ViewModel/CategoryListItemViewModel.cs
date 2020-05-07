namespace TodoApp2.Core
{
    public class CategoryListItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ListOrder { get; set; } = 0;
        public bool Trashed { get; set; } = false;

        // Not in database
        public bool IsSelected { get; set; }
    }
}
