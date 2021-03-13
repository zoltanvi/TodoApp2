using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Media;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowViewModel m_WindowViewModel;
        private bool m_Light = false;
        public MainWindow()
        {
            InitializeComponent();
            m_WindowViewModel = new WindowViewModel(this);
            DataContext = m_WindowViewModel;
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

        private void ChangeTheme(string oldTheme, string newTheme)
        {
            Uri oldUri = new Uri(oldTheme, UriKind.RelativeOrAbsolute);
            Uri newUri = new Uri(newTheme, UriKind.RelativeOrAbsolute);

            SearchAndReplaceAll(Application.Current.Resources, oldUri, newUri);
            m_WindowViewModel.NotifyThemeChanged();
        }

        private void SearchAndReplaceAll(ResourceDictionary rootDictionary, Uri oldDictionary, Uri newDictionary)
        {
            if (rootDictionary.MergedDictionaries.Count > 0)
            {
                ResourceDictionary foundDictionary = rootDictionary.MergedDictionaries
                    .FirstOrDefault(i => oldDictionary.AbsoluteUri.EndsWith(i.Source.OriginalString));
                
                if(foundDictionary != null)
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