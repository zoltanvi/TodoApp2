namespace TodoApp2.Core
{
    /// <summary>
    /// A class that implements this interface is able to
    /// get or set specific type of properties on an object.
    ///
    /// The value to set the property is always got as a string,
    /// but the implementer class should parse it
    /// to the specific data type before setting the property.
    /// </summary>
    public interface IPropertyValueHandler
    {
        /// <summary>
        /// Sets a property to the provided value.
        /// </summary>
        /// <param name="propertySource">The object which has the property.</param>
        /// <param name="name">The name of the property to set.</param>
        /// <param name="value">The value to set as string.</param>
        /// <returns>Returns true if setting the property was successful.</returns>
        bool SetProperty(object propertySource, string name, string value);

        /// <summary>
        /// Gets the value of a property as string.
        /// </summary>
        /// <param name="propertySource">The object which has the property.</param>
        /// <param name="name">The name of the property to get.</param>
        /// <returns>Returns the property value as string.</returns>
        string GetProperty(object propertySource, string name);
    }
}
