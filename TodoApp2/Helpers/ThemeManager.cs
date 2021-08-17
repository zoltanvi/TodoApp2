using System;
using System.Linq;
using System.Windows;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class ThemeManager
    {
        private const string s_AbsoluteThemePathPrefix = "pack://application:,,,/TodoApp2;component/Styles/Themes/";

        public Theme CurrentTheme { get; private set; }

        /// <summary>
        /// Changes the current theme to the provided one.
        /// </summary>
        /// <param name="oldTheme">The theme to change from.</param>
        /// <param name="newTheme">The theme to change to.</param>
        public Theme ChangeToTheme(Theme oldTheme, Theme newTheme)
        {
            if (oldTheme != newTheme)
            {
                // Get the resource paths
                var oldThemePath = GetThemeUriPath(oldTheme);
                var newThemePath = GetThemeUriPath(newTheme);

                // Change the old theme to the new one
                ChangeTheme(oldThemePath, newThemePath);
            }

            CurrentTheme = newTheme;

            return newTheme;
        }

        private string GetThemeUriPath(Theme theme)
        {
            string fullPath = s_AbsoluteThemePathPrefix;
            switch (theme)
            {
                case Theme.Darker:
                {
                    fullPath += "DarkerTheme.xaml";
                    break;
                }
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
                case Theme.White:
                {
                    fullPath += "WhiteTheme.xaml";
                    break;
                }
                //case Theme.Beige:
                //{
                //    fullPath += "BeigeTheme.xaml";
                //    break;
                //}
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