using System;
using System.Collections.Generic;
using TodoApp2.Core.Helpers;

namespace TodoApp2.Core
{
    public class ApplicationSettings
    {
        private Dictionary<string, Type> PropertyDescriptorList { get; }

        #region Properties

        public int WindowLeftPos { get; set; }
        public int WindowTopPos { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string CurrentCategory { get; set; }

        #endregion


        public ApplicationSettings()
        {
            // This dictionary describes the data that is stored in database in the Settings table
            PropertyDescriptorList = new Dictionary<string, Type>
            {
                { nameof(WindowLeftPos), typeof(int) },
                { nameof(WindowTopPos), typeof(int) },
                { nameof(WindowWidth), typeof(int) },
                { nameof(WindowHeight), typeof(int) },
                { nameof(CurrentCategory), typeof(string) },
            };
        }

        /// <summary>
        /// Loads every Settings entry from the provided list.
        /// </summary>
        /// <param name="entryList">The settings list to load.</param>
        public void LoadEntries(List<SettingsModel> entryList)
        {
            foreach (SettingsModel entry in entryList)
            {
                string currentPropertyName = entry.Key;
                if (IsEntryNameValid(currentPropertyName))
                {
                    PropertyDescriptorList.TryGetValue(currentPropertyName, out Type currentPropertyType);

                    if (currentPropertyType == typeof(int))
                    {
                        LoadInt(currentPropertyName, entry.Value);
                    }
                    else if (currentPropertyType == typeof(string))
                    {
                        LoadString(currentPropertyName, entry.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the settings list created from the properties.
        /// </summary>
        /// <returns></returns>
        public List<SettingsModel> GetEntries()
        {
            List<SettingsModel> entryList = new List<SettingsModel>();

            foreach (var propertyDescriptor in PropertyDescriptorList)
            {
                var propertyName = propertyDescriptor.Key;
                var propertyType = propertyDescriptor.Value;

                if (propertyType == typeof(int))
                {
                    entryList.Add(new SettingsModel
                    {
                        Key = propertyName,
                        Value = this.GetPropertyValue<int>(propertyName).ToString()
                    });
                } 
                else if (propertyType == typeof(string))
                {
                    entryList.Add(new SettingsModel
                    {
                        Key = propertyName,
                        Value = this.GetPropertyValue<string>(propertyName)
                    });
                }
            }

            return entryList;
        }

        /// <summary>
        /// Checks whether the entry name is a valid property or not.
        /// </summary>
        /// <param name="entryName">The entry name.</param>
        /// <returns>Returns true if the name is valid, false otherwise.</returns>
        private bool IsEntryNameValid(string entryName)
        {
            return PropertyDescriptorList.ContainsKey(entryName);
        }

        /// <summary>
        /// Loads the provided int value into the described property via reflection.
        /// </summary>
        /// <param name="propertyName">The name of the property to load into.</param>
        /// <param name="propertyValue">The property value to load.</param>
        private void LoadInt(string propertyName, string propertyValue)
        {
            if (int.TryParse(propertyValue, out int parsedValue))
            {
                this.SetPropertyValue(propertyName, parsedValue);
            }
        }

        /// <summary>
        /// Loads the provided string value into the described property via reflection.
        /// </summary>
        /// <param name="propertyName">The name of the property to load into.</param>
        /// <param name="propertyValue">The property value to load.</param>
        private void LoadString(string propertyName, string propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
        }
    }
}
