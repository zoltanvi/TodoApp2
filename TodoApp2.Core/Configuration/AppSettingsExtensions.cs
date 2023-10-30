using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace TodoApp2.Core
{
    public static class AppSettingsExtensions
    {
        public static void Read(this AppSettings appSettings, List<Setting> settingsList)
        {
            foreach (var setting in settingsList)
            {
                SetPropertyValue(appSettings, setting.Key, setting.Value);
            }
        }

        public static List<Setting> Write(this AppSettings appSettings)
        {
            return CreateSettingsList(appSettings, string.Empty);
        }

        private static List<Setting> CreateSettingsList(object obj, string currentPath)
        {
            var settings = new List<Setting>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.CanRead && prop.CanWrite) // Check if the property has both a getter and a setter
                {
                    var propValue = prop.GetValue(obj);

                    if (propValue != null)
                    {
                        string propPath = string.IsNullOrEmpty(currentPath) ? prop.Name : $"{currentPath}.{prop.Name}";

                        if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType.IsEnum)
                        {
                            var isDouble = prop.PropertyType == typeof(double);
                            
                            var propValueString = isDouble 
                                ? ((double)propValue).ToString(CultureInfo.InvariantCulture)  
                                : propValue.ToString();

                            settings.Add(new Setting(propPath, propValueString));
                        }
                        else
                        {
                            settings.AddRange(CreateSettingsList(propValue, propPath));
                        }
                    }
                }
            }

            return settings;
        }

        private static void SetPropertyValue(object obj, string path, string value)
        {
            string[] parts = path.Split('.');
            object currentObj = obj;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                PropertyInfo propInfo = currentObj.GetType().GetProperty(part);

                if (propInfo == null)
                {
#if DEBUG
                    throw new ArgumentException($"Property '{part}' not found in {currentObj.GetType().Name}");
#endif
                }

                if (i == parts.Length - 1)
                {
                    // Last part of the path - set the value
                    Type propType = propInfo.PropertyType;

                    TryConvert(value, propType, out object typedValue);

                    //object typedValue = Convert.ChangeType(value, propType);
                    propInfo.SetValue(currentObj, typedValue);
                }
                else
                {
                    // Not the last part - check for null and create an instance if necessary
                    object propValue = propInfo.GetValue(currentObj);
                    if (propValue == null)
                    {
                        propValue = Activator.CreateInstance(propInfo.PropertyType);
                        propInfo.SetValue(currentObj, propValue);
                    }

                    currentObj = propValue;
                }
            }
        }

        private static void TryConvert(string value, Type propType, out object result)
        {
            result = null;

            if (propType == typeof(string))
            {
                result = value;
            }
            else if (propType.IsEnum)
            {
                object enumValue = Enum.Parse(propType, value);

                if (Enum.IsDefined(propType, enumValue))
                {
                    result = enumValue;
                }
                else
                {
                    throw new ArgumentException($"Cannot convert unknown datatype. The datatype: [{propType.Name}].");
                }
            }
            else if (propType == typeof(double) &&
                double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleValue))
            {
                result = doubleValue;
            }
            else if (propType == typeof(int) && int.TryParse(value, out var intValue))
            {
                result = intValue;
            } 
            else if(propType == typeof(bool) && bool.TryParse(value, out var boolValue))
            {
                result = boolValue;
            }
            else
            {
                throw new ArgumentException($"Cannot convert unknown datatype. The datatype: [{propType.Name}].");
            }
        }
    }
}
