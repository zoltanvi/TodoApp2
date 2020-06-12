using System;


namespace TodoApp2.Core
{
    public class StringPropertyValueHandler : IPropertyValueHandler
    {
        /// <inheritdoc cref="IPropertyValueHandler.SetProperty"/>
        public bool SetProperty(object propertySource, string name, string value)
        {
            try
            {
                propertySource.SetPropertyValue(name, value);
            }
            catch (Exception)
            {
                // Could not set the property value
                return false;
            }
            // Success
            return true;
        }

        /// <inheritdoc cref="IPropertyValueHandler.GetProperty"/>
        public string GetProperty(object propertySource, string name)
        {
            try
            {
                return propertySource.GetPropertyValue<string>(name);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
