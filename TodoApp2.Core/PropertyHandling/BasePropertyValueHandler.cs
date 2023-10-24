using System;

namespace TodoApp2.Core
{
    public abstract class BasePropertyValueHandler<T> : IPropertyValueHandler
    {
        public string GetProperty(object propertySource, string name)
        {
            try
            {
                return DataToString(propertySource.GetPropertyValue<T>(name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SetProperty(object propertySource, string name, string value)
        {
            if (TryParseValue(value, out T parsedValue))
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

        protected abstract bool TryParseValue(string value, out T result);
        
        protected virtual string DataToString(T value) => value.ToString();
    }
}
