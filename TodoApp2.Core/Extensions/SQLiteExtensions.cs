using System;
using System.Data.SQLite;

namespace TodoApp2.Core.Extensions
{
    public static class SQLiteExtensions
    {
        public static int SafeGetInt(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }

            throw new NullReferenceException($"The record is null in the {columnName} column!");
        }

        public static long SafeGetLong(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt64(ordinal);
            }

            throw new NullReferenceException($"The record is null in the {columnName} column!");
        }

        public static long SafeGetLongFromString(this SQLiteDataReader reader, string columnName)
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

        public static int? SafeGetNullableInt(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }

            return null;
        }

        public static string SafeGetString(this SQLiteDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetString(ordinal);
            }

            return null;
        }

        public static bool SafeGetBoolFromInt(this SQLiteDataReader reader, string columnName)
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