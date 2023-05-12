﻿using PropertyChanged;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage<SettingsPageViewModel>, INotifyPropertyChanged
    {
        public SettingsPage(SettingsPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            mouseWheelEventArgs.RoutedEvent = ScrollViewer.MouseWheelEvent;
            mouseWheelEventArgs.Source = sender;
            OuterScrollViewer.RaiseEvent(mouseWheelEventArgs);
        }
    }
}