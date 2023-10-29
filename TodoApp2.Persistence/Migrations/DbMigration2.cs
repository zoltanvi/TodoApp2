using TodoApp2.Entity.Migration;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.Migrations
{
    internal class DbMigration2 : DbMigration
    {
        public DbMigration2() : base(version: 2)
        {
        }

        public override void Up()
        {
            MigrationBuilder
                .AddProperty<Task>(x => x.BorderColor, Constants.Transparent);
        }
    }
}
