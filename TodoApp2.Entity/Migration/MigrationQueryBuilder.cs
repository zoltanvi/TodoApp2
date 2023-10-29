using TodoApp2.Entity.Info;

namespace TodoApp2.Entity.Migration
{
    internal static class MigrationQueryBuilder
    {
        internal static string BuildAlterTable(PropInfo propInfo)
        {
            string query = $"ALTER TABLE {propInfo.ParentTypeName} " +
                $"ADD COLUMN {propInfo.PropName} ";  //TEXT DEFAULT \"Transparent\" ";

            if (propInfo.Type == typeof(bool)) query += "INTEGER ";
            else if (propInfo.Type == typeof(int)) query += "INTEGER ";
            else if (propInfo.Type == typeof(long)) query += "INTEGER ";
            else if (propInfo.Type == typeof(string)) query += "TEXT ";

            if (propInfo.DefaultValue != null)
            {
                // string must be in apostrophes marks
                string defaultValue = propInfo.Type == typeof(string)
                    ? $"'{propInfo.DefaultValue}'"
                    : propInfo.DefaultValue;

                query += $"DEFAULT ({defaultValue})";
            }

            query += " ;";

            return query;
        }
    }
}
