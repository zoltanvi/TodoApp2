﻿namespace TodoApp2.Core
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
        /// The overlay background has closed
        /// </summary>
        /// TODO: Remove later
        OverlayBackgroundClosed,

        /// <summary>
        /// The application should flash (orange in windows taskbar)
        /// </summary>
        WindowFlashRequested,

        /// <summary>
        /// Always on top changed, the overlay page should be closed
        /// </summary>
        AlwaysOnTopChanged,
    }
}