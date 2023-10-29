using System.Linq;
using System.Text;
using TodoApp2.Entity.Extensions;

namespace TodoApp2.Entity.Query
{
    internal static class QueryBuilder
    {
        public const string TurnForeignKeysOn = "PRAGMA foreign_keys = ON;";
        public const string GetDbVersion = "PRAGMA user_version; ";

        public static string BuildDropTable(string tableName) => $"DROP TABLE IF EXISTS {tableName} ;";
        public static string BuildUpdateDbVersion(int dbVersion) => $"PRAGMA user_version = {dbVersion}; ";

        public static string BuildCreateIfNotExists(BaseDbSetModelBuilder modelBuilder)
        {
            StringBuilder sb = new StringBuilder($"CREATE TABLE IF NOT EXISTS {modelBuilder.TableName} ( \n");
            var properties = modelBuilder.Properties;

            for (int i = 0; i < properties.Count; i++)
            {
                var item = properties[i];

                sb.Append($" {item.PropName} ");

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

            var foreignKeys = modelBuilder.ForeignKeys;
            
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

        public static string BuildInsertInto<TModel>(string tableName, string primaryKeyName)
            where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var properties = modelType.GetPublicPropertiesWithExclusion(primaryKeyName);

            StringBuilder sb = new StringBuilder($"INSERT INTO {tableName} (\n");

            for (int i = 0; i < properties.Count; i++)
            {
                sb.Append($"{properties[i].Name}");

                if (i != properties.Count - 1) sb.AppendLine(", ");
            }

            sb.Append(" ) \nVALUES (\n");

            for (int i = 0; i < properties.Count; i++)
            {
                // @ prefix
                sb.Append($"@{properties[i].Name}");

                if (i != properties.Count - 1) sb.AppendLine(", ");
            }

            sb.Append(" );");
            return sb.ToString();
        }
    }
}
