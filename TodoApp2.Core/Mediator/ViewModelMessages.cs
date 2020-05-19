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
        CategoryChanged = 1,

        /// <summary>
        /// The reminder setter panel should be opened
        /// </summary>
        OpenReminder = 2,

        /// <summary>
        /// The overlay background has closed
        /// </summary>
        OverlayBackgroundClosed = 4,
    }
}
