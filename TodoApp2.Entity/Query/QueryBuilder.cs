using System;
using System.Linq;
using System.Text;
using TodoApp2.Entity.Extensions;
using TodoApp2.Entity.Model;

namespace TodoApp2.Entity.Query
{
    internal static class QueryBuilder
    {
        public const int NoLimit = -1;
        public const int Single = 1;

        public const string TurnForeignKeysOn = "PRAGMA foreign_keys = ON;";
        public const string GetDbVersion = "PRAGMA user_version; ";

        public static string UpdateDbVersion(int dbVersion) => $"PRAGMA user_version = {dbVersion}; ";
        public static string DropTable(string tableName) => $"DROP TABLE IF EXISTS {tableName} ;";
        internal static string SelectCount(string tableName) => $"SELECT COUNT(*) AS Value FROM {tableName} ;";
        
        internal static string SelectAll(string tableName, string whereSqlCondition = null, int limit = NoLimit)
        {
            string query = $"SELECT * FROM {tableName} ";

            if (!string.IsNullOrWhiteSpace(whereSqlCondition))
            {
                query += $"WHERE {whereSqlCondition} ";
            }

            if (limit < NoLimit) throw new ArgumentOutOfRangeException("Can't return less than 0 items!");

            if (limit != NoLimit)
            {
                query += $"LIMIT {limit}";
            }

            return query;
        } 

        public static string CreateIfNotExists(BaseDbSetModelBuilder modelBuilder)
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

        public static string InsertInto<TModel>(string tableName, string primaryKeyName)
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
