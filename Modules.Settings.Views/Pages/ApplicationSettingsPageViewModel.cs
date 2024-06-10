using Modules.Common.Database;
using Modules.Common.DataBinding;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Diagnostics;
using System.Windows.Input;

namespace Modules.Settings.Views.Pages;

[AddINotifyPropertyChangedInterface]
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
            Process.Start("explorer.exe", $@"/select,""{DbConfiguration.DatabasePath}""");
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Can't open database location.");
        }
    }
}
