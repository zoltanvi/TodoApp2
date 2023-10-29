using TodoApp2.Entity.Migration;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.Migrations
{
    internal class DbMigration3 : DbMigration
    {
        public DbMigration3() : base(version: 3)
        {
        }

        public override void Up()
        {
            MigrationBuilder
                .AddModel<Note>()
                .Property(x => x.Id, isPrimaryKey: true)
                .Property(x => x.Title)
                .Property(x => x.ListOrder, defaultValue: Constants.DefaultListOrder)
                .Property(x => x.Content)
                .Property(x => x.CreationDate)
                .Property(x => x.ModificationDate)
                .Property(x => x.Trashed, defaultValue: Constants.Zero);
        }
    }
}
