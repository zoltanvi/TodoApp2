using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class ThemeManager
    {
        private const string s_AbsoluteThemePathPrefix = "pack://application:,,,/TodoApp2;component/Styles/";
        private ApplicationSettings ApplicationSettings => IoC.Application.ApplicationSettings;

        /// <summary>
        /// Changes the current theme to the provided one.
        /// </summary>
        /// <param name="theme">The theme to change to.</param>
        public Theme ChangeToTheme(Theme oldTheme, Theme newTheme)
        {
            // Get the resource paths
            var oldThemePath = GetThemeUriPath(oldTheme);
            var newThemePath = GetThemeUriPath(newTheme);

            // Change the old theme to the new one
            ChangeTheme(oldThemePath, newThemePath);

            return newTheme;
        }

        /// <summary>
        /// Changes the current theme to the next one in order.
        /// The theme order is defined by the values in <see cref="Theme"/> enum.
        /// </summary>
        public Theme ChangeToNextTheme()
        {
            // Get the list of Theme values in order to calculate the next value
            List<Theme> themeValues = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();

            // Calculate the next Theme
            int nextThemeIndex = (themeValues.IndexOf(ApplicationSettings.ActiveTheme) + 1) % themeValues.Count;
            Theme nextTheme = themeValues[nextThemeIndex];

            return ChangeToTheme(ApplicationSettings.ActiveTheme, nextTheme);
        }

        private string GetThemeUriPath(Theme theme)
        {
            string fullPath = s_AbsoluteThemePathPrefix;
            switch (theme)
            {
                case Theme.Dark:
                {
                    fullPath += "DarkTheme.xaml";
                    break;
                }
                case Theme.Light:
                {
                    fullPath += "LightTheme.xaml";
                    break;
                }
                default:
                {
                    throw new ArgumentException("The given theme is not yet defined!");
                }
            }
            return fullPath;
        }

        private void ChangeTheme(string oldTheme, string newTheme)
        {
            Uri oldUri = new Uri(oldTheme, UriKind.RelativeOrAbsolute);
            Uri newUri = new Uri(newTheme, UriKind.RelativeOrAbsolute);

            // The Application resources contains all resources that are used
            SearchAndReplaceAll(Application.Current.Resources, oldUri, newUri);
        }

        private void SearchAndReplaceAll(ResourceDictionary rootDictionary, Uri oldDictionary, Uri newDictionary)
        {
            if (rootDictionary.MergedDictionaries.Count > 0)
            {
                ResourceDictionary foundDictionary = rootDictionary.MergedDictionaries
                    .FirstOrDefault(i => oldDictionary.AbsoluteUri.EndsWith(i.Source.OriginalString));

                if (foundDictionary != null)
                {
                    ResourceDictionary newRes = new ResourceDictionary() { Source = newDictionary };
                    int index = rootDictionary.MergedDictionaries.IndexOf(foundDictionary);
                    rootDictionary.MergedDictionaries.RemoveAt(index);
                    rootDictionary.MergedDictionaries.Insert(index, newRes);
                }

                foreach (ResourceDictionary item in rootDictionary.MergedDictionaries)
                {
                    SearchAndReplaceAll(item, oldDictionary, newDictionary);
                }
            }
        }
    }
}