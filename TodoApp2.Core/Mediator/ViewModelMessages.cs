namespace TodoApp2.Core
{
    /// <summary>
    /// Defines the available message types for the mediator and it's clients
    /// </summary>
    public enum ViewModelMessages
    {
        /// <summary>
        /// The selected category changed
        /// </summary>
        CategoryChanged,

        /// <summary>
        /// The application should flash (orange in windows taskbar)
        /// </summary>
        WindowFlashRequested,

        /// <summary>
        /// The opened notification closed
        /// </summary>
        NotificationClosed,

        /// <summary>
        /// The Application theme changed
        /// </summary>
        ThemeChanged,

        /// <summary>
        /// The selected note changed
        /// </summary>
        NoteChanged,

        /// <summary>
        /// The navigator button (top left on the title bar) has been clicked
        /// </summary>
        SideMenuButtonClicked,

        /// <summary>
        /// Closing of the side menu has been requested
        /// </summary>
        SideMenuCloseRequested,

        /// <summary>
        /// Ctrl + Shift + L has been pressed in a RichTextBox
        /// </summary>
        NextThemeWithHotkeyRequested,

        /// <summary>
        /// Task page's bottom text editor should be focused
        /// </summary>
        FocusBottomTextEditor,
    }
}