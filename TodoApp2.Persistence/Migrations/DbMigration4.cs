using TodoApp2.Entity.Migration;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.Migrations
{
    internal class DbMigration4 : DbMigration
    {
        public DbMigration4() : base(version: 4)
        {
        }

        public override void Up()
        {
            MigrationBuilder
                .AddProperty<Task>(x => x.BackgroundColor, Constants.Transparent);
        }
    }
}
