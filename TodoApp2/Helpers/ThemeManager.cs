using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    internal class ThemeManager
    {
        private const string s_AbsoluteThemePathPrefix = "pack://application:,,,/TodoApp2;component/Styles/Themes/";

        public Theme CurrentTheme { get; private set; }
        public ResourceDictionary CurrentThemeDictionary { get; private set; }
        public List<Theme> ThemeList { get; }
        public event EventHandler ThemeChanged;
        private Dictionary<Theme, IColorScheme> m_ColorSchemes;
        public ThemeManager()
        {
            ThemeList = new List<Theme>();
            Array values = Enum.GetValues(typeof(Theme));

            foreach (Theme item in values)
            {
                ThemeList.Add(item);
            }

            m_ColorSchemes = new Dictionary<Theme, IColorScheme>()
            {
                //{Theme.VSBlue, new VSBlueColorScheme() },
                //{Theme.Yellow, new YellowColorScheme() }
            };
        }

        /// <summary>
        /// Changes the current theme to the provided one.
        /// </summary>
        /// <param name="oldTheme">The theme to change from.</param>
        /// <param name="newTheme">The theme to change to.</param>
        public Theme ChangeToTheme(Theme oldTheme, Theme newTheme)
        {
            string newThemePath = GetThemeUriPath(newTheme);
            Uri newUri = new Uri(newThemePath, UriKind.RelativeOrAbsolute);
            CurrentThemeDictionary = new ResourceDictionary() { Source = newUri };

            if (oldTheme != newTheme)
            {
                string oldThemePath = GetThemeUriPath(oldTheme);

                // Change the old theme to the new one
                ChangeTheme(oldThemePath, newThemePath);
            }

            if (m_ColorSchemes.ContainsKey(newTheme))
            {
                //m_ColorSchemes[newTheme].ChangeColors();
            }

            //if (newTheme == Theme.VSBlue)
            //{
            //    RedBorder redBorder = new RedBorder();
            //    redBorder.ChangeColor();
            //}

            CurrentTheme = newTheme;
            ThemeChanged?.Invoke(this, EventArgs.Empty);
            Mediator.NotifyClients(ViewModelMessages.ThemeChanged);

            return newTheme;
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

        private string GetThemeUriPath(Theme theme)
        {
            string fullPath = $"{s_AbsoluteThemePathPrefix}{theme}Theme.xaml";

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