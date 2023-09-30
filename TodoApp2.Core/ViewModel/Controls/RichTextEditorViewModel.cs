using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public class RichTextEditorViewModel : BaseViewModel
    {
        private bool m_IsEditMode;
        private bool m_EnterActionOnLostFocus;
        private bool m_ToolbarCloseOnLostFocus;

        public bool Focusable { get; set; }
        public bool NeedFocus { get; set; }
        public bool AcceptsTab { get; set; }
        public bool IsFormattedPasteEnabled => IoC.AppSettings.IsFormattedPasteEnabled;
        public string WatermarkText { get; set; }
        public bool IsEditMode
        {
            get => m_IsEditMode;
            set
            {
                m_IsEditMode = value;
                if (FocusOnEditMode)
                {
                    Focusable = value;
                    NeedFocus = value;
                }
            }
        }

        public bool FocusOnEditMode { get; set; }
        public bool IsContentEmpty { get; set; }
        public string DocumentContent { get; set; }
        public bool IsToolbarOpen { get; set; }
        public bool IsDisplayMode => !IsEditMode;
        public string TextColor { get; set; } = GlobalConstants.ColorName.Transparent;
        public double TextOpacity { get; set; } = 1.0;
        public Action EnterAction { get; set; }
        public ICommand LostFocusCommand { get; }

        public RichTextEditorViewModel(bool focusOnEditMode, bool enterActionOnLostFocus, bool toolbarCloseOnLostFocus, bool acceptsTab)
        {
            m_EnterActionOnLostFocus = enterActionOnLostFocus;
            m_ToolbarCloseOnLostFocus = toolbarCloseOnLostFocus;
            Focusable = true;
            FocusOnEditMode = focusOnEditMode;
            LostFocusCommand = new RelayCommand(OnLostFocus);
            AcceptsTab = acceptsTab;
        }

        private void OnLostFocus()
        {
            if (m_ToolbarCloseOnLostFocus)
            {
                IsToolbarOpen = false;
            }

            if (m_EnterActionOnLostFocus)
            {
                EnterAction?.Invoke();

                IsEditMode = false;
            }
        }
    }
}
