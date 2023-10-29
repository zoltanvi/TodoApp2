using TodoApp2.Entity;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.TableMappings
{
    internal class SettingsTableMapping : DbSetMapping<Setting>
    {
        public SettingsTableMapping() : base(Constants.TableNames.Settings)
        {
            ModelBuilder
                .Property(x => x.Key, isPrimaryKey: true)
                .Property(x => x.Value);
        }
    }
}
