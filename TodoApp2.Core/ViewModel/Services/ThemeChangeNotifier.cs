using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class ThemeChangeNotifier
    {
        private AppSettings _appSettings;

        public AppSettings AppSettings
        {
            get => _appSettings;
            set
            {
                Unsubscribe();
                _appSettings = value;
                Subscribe();
            }
        }
        
        private void OnDarkModeChanged()
        {
            // Trigger a property change event to force update the dependent UI elements
            _appSettings.CommonSettings.OnPropertyChanged(nameof(CommonSettings.AppBorderColor));
        }

        private void Subscribe()
        {
            if (_appSettings != null)
            {
                _appSettings.ThemeSettings.PropertyChanged += ThemeSettings_PropertyChanged;
            }
        }

        private void Unsubscribe()
        {
            if (_appSettings != null)
            {
                _appSettings.ThemeSettings.PropertyChanged -= ThemeSettings_PropertyChanged;
            }
        }

        private void ThemeSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnDarkModeChanged();
        }
    }
}
