using System;
using System.Collections.ObjectModel;
using System.Windows;
using TodoApp2.Core;
using Color = System.Windows.Media.Color;

namespace TodoApp2
{
    public class ThemeListService : BaseViewModel
    {
        public ObservableCollection<ThemeViewModel> Items { get; set; }

        public string ActiveTheme { get; private set; }

        public ThemeListService()
        {
            Items = new ObservableCollection<ThemeViewModel>();

            Array values = Enum.GetValues(typeof(Theme));

            foreach (Theme theme in values)
            {
                ResourceDictionary themeResources = GetResourceDictionary(theme);

                Items.Add(CreateThemeViewModel(themeResources, theme));
            }

            ActiveTheme = ThemeManager.CurrentTheme.ToString();

            Mediator.Register(OnThemeChanged, ViewModelMessages.ThemeChanged);
        }

        private void OnThemeChanged(object obj)
        {
            ActiveTheme = ThemeManager.CurrentTheme.ToString();
        }

        private ThemeViewModel CreateThemeViewModel(ResourceDictionary themeResources, Theme theme)
        {
            var viewModel = new ThemeViewModel(theme, theme.ToString());
            viewModel.Foreground = GetColorFromBrushResource(themeResources, "Foreground");
            viewModel.SidebarForeground = GetColorFromBrushResource(themeResources, "Foreground");
            viewModel.SidebarBackground = GetColorFromBrushResource(themeResources, "SideMenuBg");
            viewModel.TitleBar = GetColorFromBrushResource(themeResources, "TitleBarBg");
            viewModel.TaskBackground = GetColorFromBrushResource(themeResources, "TaskBg");
            viewModel.PageBackground = GetColorFromBrushResource(themeResources, "PageBg");
            viewModel.SidebarSelectionForeground = GetColorFromBrushResource(themeResources, "SelectedCategory");
            viewModel.SidebarSelectionBackground = GetColorFromBrushResource(themeResources, "SelectedCategoryBg");

            return viewModel;
        }

        private string GetColorFromBrushResource(ResourceDictionary themeResources, string resourceName)
        {
            Color brush = (Color)themeResources[resourceName];
            return brush.ToString();
        }

        private ResourceDictionary GetResourceDictionary(Theme blueTint)
        {
            string themePath = ThemeManager.GetThemeUriPath(blueTint);
            Uri newUri = new Uri(themePath, UriKind.RelativeOrAbsolute);
            return new ResourceDictionary() { Source = newUri };
        }
    }
}
