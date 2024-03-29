﻿using System;
using System.Collections.Generic;
using System.Globalization;
using TodoApp2.Core;

namespace TodoApp2
{
    public class SettingsPageTitleConverter : BaseValueConverter
    {
        private Dictionary<ApplicationPage, string> _pageTitles = new Dictionary<ApplicationPage, string>
        {
            { ApplicationPage.AppWindowSettings, "Application settings" },
            { ApplicationPage.ThemeSettings, "Themes" },
            { ApplicationPage.PageTitleSettings, "Page title settings" },
            { ApplicationPage.TaskPageSettings, "Task page settings" },
            { ApplicationPage.TaskItemSettings, "Task item settings" },
            { ApplicationPage.TaskQuickActionsSettings, "Task quick actions" },
            { ApplicationPage.TextEditorQuickActionsSettings, "Text editor quick actions" },
            { ApplicationPage.NotePageSettings, "Note page settings" },
            { ApplicationPage.DateTimeSettings, "Date time settings" },
            { ApplicationPage.Shortcuts, "Shortcuts" },
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ApplicationPage page)
            {
                if (_pageTitles.TryGetValue(page, out string title))
                {
                    return title;
                }
            }

            return "TITLE CONVERTER ERROR";
        }
    }
}
