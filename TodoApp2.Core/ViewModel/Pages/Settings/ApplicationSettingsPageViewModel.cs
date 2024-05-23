using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TodoApp2.Core;

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
            throw new ApplicationException("Can't open database location.");
        }
    }
}
