using System;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using TodoApp2.Entity.Model;

namespace TodoApp2.Entity.Extensions
{
    public static class SQLiteDataReaderExtensions
    {
        public static TModel ReadProperties<TModel>(this SQLiteDataReader reader)
            where TModel : EntityModel, new()
        {
            Type modelType = typeof(TModel);
            TModel result = new TModel();

            // Public properties with getter and setter
            var properties = modelType.GetPublicProperties();

            foreach (PropertyInfo propInfo in properties)
            {
                object propertyValue = ReadProperty(reader, propInfo);
                propInfo.SetValue(result, propertyValue);
            }

            return result;
        }

        public static object ReadProperty(this SQLiteDataReader reader, PropertyInfo propertyInfo)
        {
            Type propType = propertyInfo.PropertyType;
            string propName = propertyInfo.Name;

            if (propType == typeof(bool)) return reader.SafeGetBoolFromInt(propName);
            if (propType == typeof(int)) return reader.SafeGetInt(propName);
            if (propType == typeof(long)) return reader.SafeGetLong(propName);
            if (propType == typeof(string)) return reader.SafeGetString(propName);

            throw new ArgumentException($"Cannot read unknown type [{propType.Name}]");
        }

        private static int SafeGetInt(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }

            throw new NullReferenceException($"The record is null in the {columnName} column!");
        }

        private static int? SafeGetNullableInt(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }

            return null;
        }

        private static long SafeGetLong(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt64(ordinal);
            }

            throw new NullReferenceException($"The record is null in the {columnName} column!");
        }

        private static string SafeGetString(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetString(ordinal);
            }

            return null;
        }

        private static bool SafeGetBoolFromInt(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return Convert.ToBoolean(reader.GetInt32(ordinal));
            }

            throw new NullReferenceException($"The record is null in the {columnName} column!");
        }
    }
}
