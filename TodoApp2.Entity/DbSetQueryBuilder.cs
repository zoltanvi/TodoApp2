using System.Text;

namespace TodoApp2.Entity
{
    internal static  class DbSetQueryBuilder
    {
        public static string BuildCreateIfNotExists<T>(DbSet<T> dbSet) where T : class, new()
        {
            StringBuilder sb = new StringBuilder($"CREATE TABLE IF NOT EXISTS {dbSet.TableName} ( \n");
            var properties = dbSet.DbSetConfiguration.Properties;

            for (int i = 0; i < properties.Count; i++)
            {
                var item = properties[i];

                sb.Append($" {item.Name} ");

                if (item.Type == typeof(bool)) sb.Append("INTEGER ");
                else if (item.Type == typeof(int)) sb.Append("INTEGER ");
                else if (item.Type == typeof(long)) sb.Append("INTEGER ");
                else if (item.Type == typeof(string)) sb.Append("TEXT ");

                if (item.IsPrimaryKey) sb.Append("PRIMARY KEY ");
                else if (item.DefaultValue != null)
                {
                    // string must be in apostrophes marks
                    string defaultValue = item.Type == typeof(string)
                        ? $"'{item.DefaultValue}'"
                        : item.DefaultValue;

                    sb.Append($"DEFAULT ({defaultValue}) ");
                }

                if (i != properties.Count - 1) sb.AppendLine(", ");
            }

            var foreignKeys = dbSet.DbSetConfiguration.ForeignKeys;
            
            if (foreignKeys.Count != 0) sb.AppendLine(", ");
            
            for (int i = 0; i < foreignKeys.Count; i++)
            {
                var info = foreignKeys[i];
                sb.Append($"FOREIGN KEY ({info.Name}) REFERENCES ")
                    .Append($"{info.ReferencedTableName} ({info.ReferencedPropertyName}) ")
                    .Append("ON UPDATE CASCADE ON DELETE CASCADE ");

                if (i != foreignKeys.Count - 1) sb.AppendLine(", ");
            }

            sb.AppendLine(")");

            return sb.ToString();
        }
    }
}
