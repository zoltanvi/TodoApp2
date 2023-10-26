using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using TodoApp2.Core;
using TodoApp2.Material;

namespace TodoApp2
{
    public class MaterialThemeManagerService : IThemeManagerService
    {
        internal static IThemeManagerService Get(AppSettings appSettings) => _instance ?? (_instance = new MaterialThemeManagerService(appSettings));
        private static IThemeManagerService _instance;
        private AppSettings _appSettings;

        private ThemeSettings ThemeSettings => _appSettings.ThemeSettings;
        private uint SeedColor { get; set; }
        public Scheme<string> CurrentScheme { get; set; }

        private MaterialThemeManagerService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.ThemeSettings.PropertyChanged += ThemeSettings_PropertyChanged;
            
            UpdateTheme();
        }

        private void ThemeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTheme();
        }

        private void UpdateTheme()
        {
            SeedColor = MaterialColorHelper.HexToDecimal(ThemeSettings.SeedColor);
            CorePalette corePalette = CorePalette.Of(SeedColor, ThemeSettings.ThemeStyle);

            if (ThemeSettings.DarkMode)
            {
                Scheme<uint> darkScheme = new DarkSchemeMapper().Map(corePalette);
                CurrentScheme = darkScheme.Convert(x => MaterialColorHelper.DecimalToHex(x));
            }
            else
            {
                Scheme<uint> lightScheme = new LightSchemeMapper().Map(corePalette);
                CurrentScheme = lightScheme.Convert(x => MaterialColorHelper.DecimalToHex(x));
            }

            UpdateResources();
        }

        private void UpdateResources()
        {
            ResourceDictionary currentResources = Application.Current.Resources;

            foreach (KeyValuePair<string, string> item in CurrentScheme.Enumerate())
            {
                if (currentResources.Contains(item.Key))
                {
                    currentResources[item.Key] = new SolidColorBrush(MaterialColorHelper.HexToColor(item.Value));
                }
            }
        }
    }
}
