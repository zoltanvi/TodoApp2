using System.Collections.Generic;
using TodoApp2.Entity.Info;

namespace TodoApp2.Entity
{
    public abstract class BaseDbSetModelBuilder
    {
        internal abstract string TableName { get; }
        internal List<PropInfo> Properties { get; } = new List<PropInfo>();
        internal List<ForeignKeyInfo> ForeignKeys { get; } = new List<ForeignKeyInfo>();
    }
}
