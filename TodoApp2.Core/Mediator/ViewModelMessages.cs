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
        /// Online mode change has been requested.
        /// Changes should be persisted at this point because the database is going to be changed.
        /// </summary>
        OnlineModeChangeRequested,

        /// <summary>
        /// Online mode changed.
        /// The new database is ready for use.
        /// </summary>
        OnlineModeChanged
    }
}