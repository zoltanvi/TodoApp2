using System.Windows;

namespace TodoApp2;

/// <summary>
/// Interaction logic for DevSettingsWindow.xaml
/// </summary>
public partial class DevSettingsWindow : Window
{
    private DevSettingsWindowViewModel ViewModel { get; set; }

    public DevSettingsWindow()
    {
        InitializeComponent();

        ViewModel = new DevSettingsWindowViewModel();
        DataContext = ViewModel;

        Show();
    }
}
