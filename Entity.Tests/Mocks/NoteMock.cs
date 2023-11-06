using TodoApp2.Entity.Model;

namespace Entity.Tests.Mocks
{
    public class NoteMock : EntityModel
    {
        public string someStringField = "asd";
        public int someIntField = 23;
        public bool someBoolField = true;

        public int Id { get; set; }
        public string Title { get; set; }
        public string ListOrder { get; set; }
        public string Content { get; set; }
        public long CreationDate { get; set; }
        public long ModificationDate { get; set; }
        public bool Trashed { get; set; }

        public bool IsGood() => Id == 0;
    }
}
