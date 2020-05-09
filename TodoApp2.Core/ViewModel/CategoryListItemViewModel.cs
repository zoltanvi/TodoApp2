namespace TodoApp2.Core
{
    public class CategoryListItemViewModel : BaseViewModel, IReorderable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ListOrder { get; set; }
        public bool Trashed { get; set; }

        // Not in database
        public bool IsSelected { get; set; }
    }
}
