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
        /// The Application theme should change
        /// </summary>
        ThemeChangeRequested,

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

    }
}