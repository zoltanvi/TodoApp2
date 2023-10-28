using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace TodoApp2.Entity.Extensions
{
    public static class SQLiteDataReaderExtensions
    {
        private static Dictionary<Type, Func<SQLiteDataReader, string, object>> PropertyReaders 
            = new Dictionary<Type, Func<SQLiteDataReader, string, object>>
        {
            { typeof(bool), ReadBool },
            { typeof(int), ReadInt },
            { typeof(long), ReadLong },
            { typeof(string), ReadString },
        };

        public static TModel ReadProperties<TModel>(this SQLiteDataReader reader)
            where TModel : class, new()
        {
            Type modelType = typeof(TModel);

            TModel result = new TModel();

            // Public properties with getter and setter
            var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.CanRead);

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

            if (!PropertyReaders.TryGetValue(propType, out var propertyReader)) throw new ArgumentException($"Cannot read unknown type [{propType.Name}]");

            object propValue = propertyReader(reader, propertyInfo.Name);
            return propValue;
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

        // TODO: delete
        private static long SafeGetLongFromString(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                string textLong = reader.GetString(ordinal);
                if (long.TryParse(textLong, out long result))
                {
                    return result;
                }
            }

            throw new NullReferenceException($"The record is not a long in the {columnName} column!");
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

        private static object ReadBool(SQLiteDataReader reader, string propertyName) => reader.SafeGetBoolFromInt(propertyName);
        private static object ReadInt(SQLiteDataReader reader, string propertyName) => reader.SafeGetInt(propertyName);
        private static object ReadLong(SQLiteDataReader reader, string propertyName) => reader.SafeGetLong(propertyName);
        private static object ReadString(SQLiteDataReader reader, string propertyName) => reader.SafeGetString(propertyName);
    }
}
