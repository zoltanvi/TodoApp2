﻿using System.Windows;

namespace TodoApp2
{
    public class ToolbarService
    {
        private static ToolbarService _instance;
        private static SingletonToolbar _cachedToolbar;
        private static ToolbarService Instance => _instance ?? (_instance = new ToolbarService());

        public static SingletonToolbar Toolbar => Instance.GetToolbar();

        private SingletonToolbar GetToolbar()
        {
            if (_cachedToolbar == null)
            {
                _cachedToolbar = Application.Current.TryFindResource("TextEditorToolbar") as SingletonToolbar;
            }

            return _cachedToolbar;
        }
    }
}
