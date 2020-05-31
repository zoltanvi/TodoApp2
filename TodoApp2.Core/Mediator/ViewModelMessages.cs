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
        /// The reminder setter panel should be opened
        /// </summary>
        OpenReminderPageRequested,

        /// <summary>
        /// The notification page should be opened
        /// </summary>
        OpenNotificationPageRequested,

        /// <summary>
        /// The overlay background should be visible
        /// </summary>
        OpenOverlayBackgroundRequested,

        /// <summary>
        /// The overlay background has closed
        /// </summary>
        OverlayBackgroundClosed,

        /// <summary>
        /// The application should flash (orange in windows taskbar)
        /// </summary>
        WindowFlashRequested,
    }
}
