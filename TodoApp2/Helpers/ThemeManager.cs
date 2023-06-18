using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ThemeManager
    {
        private const string AbsoluteThemePathPrefix = "pack://application:,,,/TodoApp2;component/Styles/Themes/";
        private const string DarkBaseUri = "pack://application:,,,/TodoApp2;component/Styles/DarkAndLight/DarkBase.xaml";
        private const string LightBaseUri = "pack://application:,,,/TodoApp2;component/Styles/DarkAndLight/LightBase.xaml";

        private HashSet<Theme> _lightThemes;

        public static Theme CurrentTheme { get; private set; }

        public Theme ActiveTheme => CurrentTheme;

        public List<Theme> ThemeList { get; }

        public event EventHandler ThemeChanged;

        public ThemeManager()
        {
            _lightThemes = new HashSet<Theme> {
                Theme.LightGreen,
                Theme.MaterialLight,
                Theme.YellowGold,
                Theme.Mint,
                Theme.LightBlue,
                Theme.Violet,
                Theme.LightGray,
                Theme.Light
            };

            ThemeList = new List<Theme>();
            Array values = Enum.GetValues(typeof(Theme));

            foreach (Theme item in values)
            {
                ThemeList.Add(item);
            }
        }

        /// <summary>
        /// Changes the current theme to the provided one.
        /// </summary>
        /// <param name="oldTheme">The theme to change from.</param>
        /// <param name="newTheme">The theme to change to.</param>
        public Theme ChangeToTheme(Theme oldTheme, Theme newTheme)
        {
            if (oldTheme != newTheme)
            {
                SwitchLightAndDark(oldTheme, newTheme);

                string oldThemePath = GetThemeUriPath(oldTheme);
                string newThemePath = GetThemeUriPath(newTheme);

                // Change the old theme to the new one
                ChangeTheme(oldThemePath, newThemePath);
            }

            CurrentTheme = newTheme;
            ThemeChanged?.Invoke(this, EventArgs.Empty);
            Mediator.NotifyClients(ViewModelMessages.ThemeChanged);

            return newTheme;
        }

        private void SwitchLightAndDark(Theme oldTheme, Theme newTheme)
        {
            bool oldIsLight = _lightThemes.Contains(oldTheme);
            bool newIsLight = _lightThemes.Contains(newTheme);

            if (oldIsLight != newIsLight)
            {
                if (newIsLight)
                {
                    ChangeTheme(DarkBaseUri, LightBaseUri);
                }
                else
                {
                    ChangeTheme(LightBaseUri, DarkBaseUri);
                }
            }
        }

        public void UpdateTheme(string changedKey, SolidColorBrush changedValue)
        {
            ResourceDictionary currentResources = Application.Current.Resources;
            if (currentResources.Contains(changedKey))
            {
                currentResources[changedKey] = changedValue;
            }
            Mediator.NotifyClients(ViewModelMessages.ThemeChanged);
        }

        internal static string GetThemeUriPath(Theme theme)
        {
            string fullPath = $"{AbsoluteThemePathPrefix}{theme}Theme.xaml";

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