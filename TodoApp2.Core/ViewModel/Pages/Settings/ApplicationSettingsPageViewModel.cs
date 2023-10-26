using System;
using System.Windows.Input;
using System.Diagnostics;

namespace TodoApp2.Core
{
    public class ApplicationSettingsPageViewModel : BaseViewModel
    {
        public ICommand OpenDbLocationCommand { get; set; }

        public ApplicationSettingsPageViewModel()
        {
            OpenDbLocationCommand = new RelayCommand(OpenDbLocation);
        }

        private void OpenDbLocation()
        {
            try
            {
                Process.Start("explorer.exe", $@"/select,""{IoC.AppViewModel.DatabaseLocation}""");
            }
            catch (Exception ex)
            {
                // Ignore for the time being
            }
        }
    }
}
