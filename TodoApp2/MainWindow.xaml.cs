using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_Light = false;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new WindowViewModel(this);
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            const string darkTheme = "pack://application:,,,/TodoApp2;component/Styles/DarkTheme.xaml";
            const string lightTheme = "pack://application:,,,/TodoApp2;component/Styles/LightTheme.xaml";

            m_Light = !m_Light;
            string oldTheme = m_Light ? darkTheme : lightTheme;
            string newTheme = m_Light ? lightTheme : darkTheme;
            ChangeTheme(oldTheme, newTheme);
        }

        public void ChangeTheme(string oldTheme, string newTheme)
        {
            var oldUri = new Uri(oldTheme, UriKind.RelativeOrAbsolute);
            var newUri = new Uri(newTheme, UriKind.RelativeOrAbsolute);

            SearchAndReplace(Application.Current.Resources, oldUri, newUri);
        }

        private void SearchAndReplace(ResourceDictionary baseDictionary, Uri oldDictionary, Uri newDictionary)
        {
            if (baseDictionary.MergedDictionaries.Count > 0)
            {
                ResourceDictionary foundDictionary = baseDictionary.MergedDictionaries
                    .FirstOrDefault(i => oldDictionary.AbsoluteUri.EndsWith(i.Source.OriginalString));
                
                if(foundDictionary != null)
                {
                    ResourceDictionary newRes = new ResourceDictionary() { Source = newDictionary };
                    int index = baseDictionary.MergedDictionaries.IndexOf(foundDictionary);
                    baseDictionary.MergedDictionaries.RemoveAt(index);
                    baseDictionary.MergedDictionaries.Insert(index, newRes);
                }

                foreach (ResourceDictionary item in baseDictionary.MergedDictionaries)
                {
                    SearchAndReplace(item, oldDictionary, newDictionary);
                }
            }
        }
    }
}