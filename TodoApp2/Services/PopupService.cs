using System.Windows;

namespace TodoApp2
{
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
                _cachedPopup = Application.Current.TryFindResource("ColorPickerPopup") as SingletonPopup;
            }

            return _cachedPopup;
        }
    }
}
