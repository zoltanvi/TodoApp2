using TodoApp2.Entity;
using TodoApp2.Persistence.Models;

namespace TodoApp2.Persistence.TableMappings
{
    internal class CategoryTableMapping : DbSetMapping<Category>
    {
        public CategoryTableMapping() : base(Constants.TableNames.Category)
        {
            ModelBuilder
               .Property(x => x.Id, isPrimaryKey: true)
               .Property(x => x.Name)
               .Property(x => x.ListOrder, defaultValue: Constants.DefaultListOrder)
               .Property(x => x.Trashed);
        }
    }
}
