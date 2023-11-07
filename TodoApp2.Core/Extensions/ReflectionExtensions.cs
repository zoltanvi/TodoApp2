using System;
using System.Reflection;

namespace TodoApp2.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            ThrowIfNull(obj);

            return GetFieldValue<T>(obj, obj.GetType(), fieldName);
        }

        public static T GetFieldValue<T>(this object obj, Type objectType, string fieldName)
        {
            ThrowIfNull(obj);

            object value = objectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(obj);

            return value is T ? (T)value : default(T);
        }

        public static void SetFieldValue<T>(this object obj, string fieldName, T value)
        {
            ThrowIfNull(obj);

            SetFieldValue(obj, obj.GetType(), fieldName, value);
        }

        public static void SetFieldValue<T>(this object obj, Type objectType, string fieldName, T value)
        {
            ThrowIfNull(obj);

            objectType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(obj, value);
        }

        public static T GetNonPublicPropertyValue<T>(this object obj, string propertyName)
        {
            ThrowIfNull(obj);

            return GetNonPublicPropertyValue<T>(obj, obj.GetType(), propertyName);
        }

        public static T GetNonPublicPropertyValue<T>(this object obj, Type objectType, string propertyName)
        {
            ThrowIfNull(obj);

            PropertyInfo property = objectType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

            object value = property?.GetValue(obj);

            return value is T ? (T)value : default(T);
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            ThrowIfNull(obj);

            return GetPropertyValue<T>(obj, obj.GetType(), propertyName);
        }

        public static T GetPropertyValue<T>(this object obj, Type objectType, string propertyName)
        {
            ThrowIfNull(obj);

            object value = objectType.GetProperty(propertyName)?.GetValue(obj);

            return value is T ? (T)value : default(T);
        }

        public static void SetPropertyValue<T>(this object obj, string propertyName, T value)
        {
            ThrowIfNull(obj);

            SetPropertyValue(obj, obj.GetType(), propertyName, value);
        }

        public static void SetPropertyValue<T>(this object obj, Type objectType, string propertyName, T value)
        {
            ThrowIfNull(obj);

            objectType.GetProperty(propertyName)?.SetValue(obj, value);
        }

        private static void ThrowIfNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Can't use reflection on a null object!");
            }
        }
    }
}