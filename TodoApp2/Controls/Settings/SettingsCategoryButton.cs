using System.Windows;
using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    public class SettingsCategoryButton : Button
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(ApplicationPage), typeof(SettingsCategoryButton));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(SettingsCategoryButton));
        public static readonly DependencyProperty SelectedPageProperty = DependencyProperty.Register(nameof(SelectedPage), typeof(ApplicationPage), typeof(SettingsCategoryButton), new PropertyMetadata(OnSelectedPageChanged));

        public ApplicationPage Id
        {
            get { return (ApplicationPage)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public ApplicationPage SelectedPage
        {
            get { return (ApplicationPage)GetValue(SelectedPageProperty); }
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
}
