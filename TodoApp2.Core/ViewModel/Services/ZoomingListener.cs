using System;

namespace TodoApp2.Core;

public class ZoomingListener
{
    private UIScaler _uiScaler;
    private AppSettings _appSettings;

    public ZoomingListener(UIScaler uiScaler, AppSettings appSettings)
    {
        ArgumentNullException.ThrowIfNull(uiScaler);
        ArgumentNullException.ThrowIfNull(appSettings);

        _uiScaler = uiScaler;
        _appSettings = appSettings;

        _uiScaler.Zoomed += OnZoomed;
        _appSettings.WindowSettings.PropertyChanged += WindowSettings_PropertyChanged;
    }

    private void WindowSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(WindowSettings.Scaling)) return;
        if (UIScaler.StaticScaleValue == _appSettings.WindowSettings.Scaling) return;

        _uiScaler.SetScaling(_appSettings.WindowSettings.Scaling);
    }

    private void OnZoomed(object sender, ZoomedEventArgs e)
    {
        if (UIScaler.StaticScaleValue == _appSettings.WindowSettings.Scaling) return;

        _appSettings.WindowSettings.Scaling = UIScaler.StaticScaleValue;

        // This is needed to trigger the font size update (fontSizeScaler)
        _appSettings.TaskSettings.OnPropertyChanged(nameof(TaskSettings.FontSize));
        _appSettings.PageTitleSettings.OnPropertyChanged(nameof(PageTitleSettings.FontSize));
    }
}
