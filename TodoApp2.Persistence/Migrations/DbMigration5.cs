using TodoApp2.Entity.Migration;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.Migrations
{
    internal class DbMigration5 : DbMigration
    {
        public DbMigration5() : base(version: 5)
        {
        }

        public override void Up()
        {
            MigrationBuilder
                .RemoveModel("Settings");
            
            MigrationBuilder
                .AddModel<Setting>()
                .Property(x => x.Key, isPrimaryKey: true)
                .Property(x => x.Value);
        }
    }
}
