namespace TodoApp2.Core
{
    public class ZoomingListener
    {
        private UIScaler _uiScaler;
        private AppSettings _appSettings;

        public ZoomingListener(UIScaler uiScaler, AppSettings appSettings)
        {
            _uiScaler = uiScaler;
            _appSettings = appSettings;

            _uiScaler.Zoomed += OnZoomed;
        }

        private void OnZoomed(object sender, ZoomedEventArgs e)
        {
            // This is needed to trigger the font size update (fontSizeScaler)
            _appSettings.TaskSettings.OnPropertyChanged(nameof(TaskSettings.FontSize));
            _appSettings.PageTitleSettings.OnPropertyChanged(nameof(PageTitleSettings.FontSize));
        }
    }
}
