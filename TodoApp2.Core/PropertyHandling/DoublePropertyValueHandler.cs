using System;
using System.Globalization;
using TodoApp2.Core.Helpers;

namespace TodoApp2.Core
{
    public class DoublePropertyValueHandler : IPropertyValueHandler
    {
        /// <inheritdoc cref="IPropertyValueHandler.SetProperty"/>
        public bool SetProperty(object propertySource, string name, string value)
        {
            if (double.TryParse(value, out double parsedValue))
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
                return propertySource.GetPropertyValue<double>(name).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
