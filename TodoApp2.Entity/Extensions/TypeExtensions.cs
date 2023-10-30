using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Entity.Extensions
{
    internal static class TypeExtensions
    {
        private static BindingFlags PublicPropertyFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type) => 
            type.GetProperties(PublicPropertyFlag)
            .Where(x => x.CanWrite && x.CanRead);

        public static List<PropertyInfo> GetPublicPropertiesWithExclusion(this Type type, string excludedPropertyName) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite)
            .Where(x => x.CanRead)
            .Where(x => x.Name != excludedPropertyName)
            .ToList();

        public static PropertyInfo GetPublicProperty(this Type type, string name)
        {
            var prop = type.GetProperty(name, PublicPropertyFlag);
            if (prop != null && prop.CanWrite && prop.CanRead)
            {
                return prop;
            }

            return null;
        }
    }
}
