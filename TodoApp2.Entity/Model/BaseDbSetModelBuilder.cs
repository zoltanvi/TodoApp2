using System.Collections.Generic;
using TodoApp2.Entity.Info;

namespace TodoApp2.Entity.Model
{
    public abstract class BaseDbSetModelBuilder
    {
        internal abstract string TableName { get; }
        internal List<PropInfo> Properties { get; } = new List<PropInfo>();
        internal List<ForeignKeyInfo> ForeignKeys { get; } = new List<ForeignKeyInfo>();

        internal string GetPrimaryKeyName()
        {
            foreach (PropInfo info in Properties)
            {
                if (info.IsPrimaryKey) return info.PropName;
            }

            return null;
        }
    }
}
