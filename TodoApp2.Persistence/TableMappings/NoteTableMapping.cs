using TodoApp2.Common;
using TodoApp2.Entity;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.TableMappings
{
    internal class NoteTableMapping : DbSetMapping<Note>
    {
        public NoteTableMapping() : base(Constants.TableNames.Note)
        {
            ModelBuilder
                .Property(x => x.Id, isPrimaryKey: true)
                .Property(x => x.Title)
                .Property(x => x.ListOrder, defaultValue: CommonConstants.DefaultListOrderString)
                .Property(x => x.Content)
                .Property(x => x.CreationDate)
                .Property(x => x.ModificationDate)
                .Property(x => x.Trashed, defaultValue: Constants.Zero);
        }
    }
}
