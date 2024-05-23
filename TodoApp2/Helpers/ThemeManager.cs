using Modules.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2;

/// <summary>
/// Responsible for switching between the dark and light resource dictionary, based on the DarkMode setting.
/// </summary>
public class ThemeManager
{
    private const string DarkTheme = "pack://application:,,,/TodoApp2;component/Themes/DarkTheme.xaml";
    private const string LightTheme = "pack://application:,,,/TodoApp2;component/Themes/LightTheme.xaml";

    public ThemeManager()
    {
        IoC.AppSettings.ThemeSettings.PropertyChanged += ThemeSettings_PropertyChanged;

        CheckAndSwitchLightAndDark();
    }

    private void ThemeSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ThemeSettings.DarkMode)) return;

        CheckAndSwitchLightAndDark();
    }

    private void CheckAndSwitchLightAndDark()
    {
        if (IoC.AppSettings.ThemeSettings.DarkMode)
        {
            ChangeTheme(from: LightTheme, to: DarkTheme);
        }
        else
        {
            ChangeTheme(from: DarkTheme, to: LightTheme);
        }

        Mediator.NotifyClients(ViewModelMessages.ThemeChanged);
    }

    private void ChangeTheme(string from, string to)
    {
        Uri oldUri = new Uri(from, UriKind.RelativeOrAbsolute);
        Uri newUri = new Uri(to, UriKind.RelativeOrAbsolute);

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