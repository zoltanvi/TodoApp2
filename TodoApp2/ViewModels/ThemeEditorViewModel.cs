using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using TodoApp2.Core;

namespace TodoApp2
{
    public class ThemeEditorEntryEventArgs : EventArgs
    {
        public string ChangedKey { get; set; }
        public Color ChangedValue { get; set; }
    }

    public class ThemeEditorEntry
    {
        private Color m_Value;
        public string Key { get; set; }

        public Color Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                Changed?.Invoke(this, new ThemeEditorEntryEventArgs { ChangedKey = Key, ChangedValue = value });
            }
        }

        public ThemeEditorEntry(string key, Color value)
        {
            Key = key;
            Value = value;
        }

        public event EventHandler<ThemeEditorEntryEventArgs> Changed;
    }

    public class ThemeEditorViewModel : BaseViewModel
    {
        private readonly ThemeManager m_ThemeManager;

        public ObservableCollection<ThemeEditorEntry> Items { get; set; }

        public ThemeEditorViewModel()
        {
        }

        internal ThemeEditorViewModel(ThemeManager themeManager)
        {
            m_ThemeManager = themeManager;
            Items = new ObservableCollection<ThemeEditorEntry>();
            m_ThemeManager.ThemeChanged += ThemeManagerOnThemeChanged;
        }

        private void ThemeManagerOnThemeChanged(object sender, EventArgs e)
        {
            Items.Clear();

            List<DictionaryEntry> entries = new List<DictionaryEntry>();
            if (m_ThemeManager.CurrentThemeDictionary != null)
            {
                foreach (DictionaryEntry dictionaryEntry in m_ThemeManager.CurrentThemeDictionary)
                {
                    entries.Add(dictionaryEntry);
                }
            }

            foreach (DictionaryEntry dictionaryEntry in entries.OrderBy(entry => entry.Key))
            {
                if (dictionaryEntry.Key is string key && dictionaryEntry.Value is Color value)
                {
                    var entry = new ThemeEditorEntry(key, value);
                    entry.Changed += OnEntryChanged;
                    Items.Add(entry);
                }
            }
        }

        private void OnEntryChanged(object sender, ThemeEditorEntryEventArgs args)
        {
            m_ThemeManager.UpdateTheme(args.ChangedKey, new SolidColorBrush(args.ChangedValue));
        }
    }
}
