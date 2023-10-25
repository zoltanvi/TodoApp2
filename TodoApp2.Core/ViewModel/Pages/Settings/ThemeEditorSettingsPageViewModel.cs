namespace TodoApp2.Core
{
    public class ThemeEditorSettingsPageViewModel : BaseViewModel
    {
        private readonly IResourceUpdater _resourceUpdater;
        private object _titleBarBgBrush;
        private object _pageBgBrush;
        private object _sideMenuBgBrush;
        private object _bottomBarBgBrush;
        private object _foregroundBrush;
        private object _taskBgBrush;
        private object _selectedCategoryBgBrush;
        private object _hoverCategoryBgBrush;

        public object TitleBarBgBrush
        {
            get => _titleBarBgBrush;
            set
            {
                _titleBarBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(TitleBarBgBrush), _titleBarBgBrush);
            }
        }

        public object PageBgBrush
        {
            get => _pageBgBrush;
            set
            {
                _pageBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(PageBgBrush), _pageBgBrush);
            }
        }

        public object SideMenuBgBrush
        {
            get => _sideMenuBgBrush;
            set
            {
                _sideMenuBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(SideMenuBgBrush), _sideMenuBgBrush);
            }
        }

        public object BottomBarBgBrush
        {
            get => _bottomBarBgBrush;
            set
            {
                _bottomBarBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(BottomBarBgBrush), _bottomBarBgBrush);
            }
        }

        public object ForegroundBrush
        {
            get => _foregroundBrush;
            set
            {
                _foregroundBrush = value;
                _resourceUpdater.UpdateResource(nameof(ForegroundBrush), _foregroundBrush);
            }
        }

        public object TaskBgBrush
        {
            get => _taskBgBrush;
            set
            {
                _taskBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(TaskBgBrush), _taskBgBrush);
            }
        }

        public object SelectedCategoryBgBrush
        {
            get => _selectedCategoryBgBrush;
            set
            {
                _selectedCategoryBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(SelectedCategoryBgBrush), _selectedCategoryBgBrush);
            }
        }

        public object HoverCategoryBgBrush
        {
            get => _hoverCategoryBgBrush;
            set
            {
                _hoverCategoryBgBrush = value;
                _resourceUpdater.UpdateResource(nameof(HoverCategoryBgBrush), _hoverCategoryBgBrush);
            }
        }

        public ThemeEditorSettingsPageViewModel()
        {
        }

        public ThemeEditorSettingsPageViewModel(AppViewModel appViewModel)
        {
            _resourceUpdater = IoC.ResourceUpdater;
            // TODO: 
            //IoC.AppSettings.PropertyChanged += ApplicationSettings_PropertyChanged;

            UpdateBrushes();
        }

        //private void ApplicationSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
            //if (e.PropertyName == nameof(ApplicationSettings.ActiveTheme))
            //{
            //    UpdateBrushes();
            //}
        //}

        private void UpdateBrushes()
        {
            _titleBarBgBrush = _resourceUpdater.GetResourceValue(nameof(TitleBarBgBrush));
            _pageBgBrush = _resourceUpdater.GetResourceValue(nameof(PageBgBrush));
            _sideMenuBgBrush = _resourceUpdater.GetResourceValue(nameof(SideMenuBgBrush));
            _bottomBarBgBrush = _resourceUpdater.GetResourceValue(nameof(BottomBarBgBrush));
            _foregroundBrush = _resourceUpdater.GetResourceValue(nameof(ForegroundBrush));
            _taskBgBrush = _resourceUpdater.GetResourceValue(nameof(TaskBgBrush));
            _selectedCategoryBgBrush = _resourceUpdater.GetResourceValue(nameof(SelectedCategoryBgBrush));
            _hoverCategoryBgBrush = _resourceUpdater.GetResourceValue(nameof(HoverCategoryBgBrush));
        }
    }
}
