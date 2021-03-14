using System.Collections.Generic;

namespace TodoApp2.Core
{
    /// <summary>
    /// The application settings that are stored in the database.
    /// </summary>
    ///
    /// <remarks>
    /// The properties and their values that are in this class are stored in the
    /// database as key-value pairs, where the key is the name of the property
    /// and the value is the value of the property as string.
    ///
    /// The reason why the properties are stored as key-value pairs is that
    /// this way the database model is always the same, but this class is
    /// extendable with new properties without changing the database.
    ///
    /// To add new properties which will automatically be stored in the database:
    ///   1. Add a new property to this class.
    ///   2. Extend <see cref="PropertyDescriptors"/> with the new property's name
    ///      and the corresponding <see cref="IPropertyValueHandler"/> to it's type.
    /// </remarks>
    public class ApplicationSettings
    {
        private Dictionary<string, IPropertyValueHandler> PropertyDescriptors { get; }

        #region Properties

        public int WindowLeftPos { get; set; }
        public int WindowTopPos { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string CurrentCategory { get; set; }
        public Theme ActiveTheme { get; set; }

        #endregion Properties

        public ApplicationSettings()
        {
            // This dictionary describes the data that is stored in database in the Settings table
            PropertyDescriptors = new Dictionary<string, IPropertyValueHandler>
            {
                { nameof(WindowLeftPos), PropertyValueHandlers.Integer },
                { nameof(WindowTopPos), PropertyValueHandlers.Integer },
                { nameof(WindowWidth), PropertyValueHandlers.Integer },
                { nameof(WindowHeight), PropertyValueHandlers.Integer },
                { nameof(CurrentCategory), PropertyValueHandlers.String },
                { nameof(ActiveTheme), PropertyValueHandlers.Theme },
            };
        }

        /// <summary>
        /// Loads every Settings entry from the provided list.
        /// </summary>
        /// <param name="settings">The settings list to load.</param>
        public void SetSettings(List<SettingsModel> settings)
        {
            foreach (SettingsModel entry in settings)
            {
                string propertyName = entry.Key;
                string propertyValue = entry.Value;
                if (IsPropertyNameValid(propertyName))
                {
                    if (PropertyDescriptors.TryGetValue(propertyName, out IPropertyValueHandler valueLoader))
                    {
                        valueLoader.SetProperty(this, propertyName, propertyValue);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the settings list created from the properties.
        /// </summary>
        /// <returns></returns>
        public List<SettingsModel> GetSettings()
        {
            List<SettingsModel> settings = new List<SettingsModel>();

            foreach (var propertyDescriptor in PropertyDescriptors)
            {
                var propertyName = propertyDescriptor.Key;
                var propertyLoader = propertyDescriptor.Value;

                settings.Add(new SettingsModel
                {
                    Key = propertyName,
                    Value = propertyLoader.GetProperty(this, propertyName)
                });
            }

            return settings;
        }

        /// <summary>
        /// Checks whether the property name is a valid property or not.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns true if the name is valid, false otherwise.</returns>
        private bool IsPropertyNameValid(string propertyName)
        {
            return PropertyDescriptors.ContainsKey(propertyName);
        }
    }
}