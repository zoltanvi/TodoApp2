using System;

namespace TodoApp2.Core
{
    public class EnumPropertyValueHandler<TEnum> : IPropertyValueHandler 
        where TEnum : struct
    {
        /// <inheritdoc cref="IPropertyValueHandler.SetProperty"/>
        public bool SetProperty(object propertySource, string name, string value)
        {
            if (Enum.TryParse(value, out TEnum parsedValue))
            {
                try
                {
                    propertySource.SetPropertyValue(name, parsedValue);
                }
                catch (Exception)
                {
                    // Could not set the property value
                    return false;
                }
                // Success
                return true;
            }
            // Could not parse the data
            return false;
        }

        /// <inheritdoc cref="IPropertyValueHandler.GetProperty"/>
        public string GetProperty(object propertySource, string name)
        {
            try
            {
                return propertySource.GetPropertyValue<TEnum>(name).ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}