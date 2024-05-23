using System.Windows;
using TodoApp2.Core;

namespace TodoApp2.Services;

public class PopupService
{
    private static PopupService _instance;
    private static SingletonPopup _cachedPopup;
    private static PopupService Instance => _instance ?? (_instance = new PopupService());

    public static SingletonPopup Popup => Instance.GetPopup();

    private SingletonPopup GetPopup()
    {
        if (_cachedPopup == null)
        {
            _cachedPopup = Application.Current.TryFindResource(CoreConstants.ResourceNames.ColorPickerPopup) as SingletonPopup;
        }

        return _cachedPopup;
    }
}
