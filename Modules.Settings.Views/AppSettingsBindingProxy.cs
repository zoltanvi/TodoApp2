using Modules.Settings.Contracts.ViewModels;
using System.Windows;

namespace Modules.Settings.Views;

public class AppSettingsBindingProxy : Freezable
{
    protected override Freezable CreateInstanceCore()
    {
        return new AppSettingsBindingProxy();
    }

    public AppSettings AppSettings
    {
        get { return (AppSettings)GetValue(AppSettingsProperty); }
        set { SetValue(AppSettingsProperty, value); }
    }

    public static readonly DependencyProperty AppSettingsProperty =
        DependencyProperty.Register(nameof(AppSettings), typeof(AppSettings), typeof(AppSettingsBindingProxy), new UIPropertyMetadata(null));
}
