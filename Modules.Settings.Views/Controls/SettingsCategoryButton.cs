using Modules.Settings.Contracts.Models;
using System.Windows;
using System.Windows.Controls;

namespace Modules.Settings.Views.Controls;

public class SettingsCategoryButton : Button
{
    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(SettingsPageType), typeof(SettingsCategoryButton));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SettingsCategoryButton));
    public static readonly DependencyProperty SelectedPageProperty = DependencyProperty.Register(nameof(SelectedPage), typeof(SettingsPageType), typeof(SettingsCategoryButton), new PropertyMetadata(OnSelectedPageChanged));

    public SettingsPageType Id
    {
        get { return (SettingsPageType)GetValue(IdProperty); }
        set { SetValue(IdProperty, value); }
    }

    public bool IsSelected
    {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
    }

    public SettingsPageType SelectedPage
    {
        get { return (SettingsPageType)GetValue(SelectedPageProperty); }
        set { SetValue(SelectedPageProperty, value); }
    }

    private static void OnSelectedPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SettingsCategoryButton button)
        {
            button.IsSelected = button.Id == button.SelectedPage;
        }
    }
}
