namespace TodoApp2.Core
{
    /// <summary>
    /// A page of the application
    /// </summary>
    public enum ApplicationPage
    {
        /// <summary>
        /// The default enum value.
        /// This is needed because using the other values 
        /// might create unnecessary viewModel instances
        /// </summary>
        Invalid,

        /// <summary>
        /// The task page
        /// </summary>
        Task,

        /// <summary>
        /// The category page
        /// </summary>
        Category,

        /// <summary>
        /// The options page (opened by cog in the side menu page)
        /// </summary>
        Settings,

        /// <summary>
        /// The reminder page (where a reminder can be set for a task)
        /// </summary>
        Reminder,

        /// <summary>
        /// The notification page (pops up when a notification occurs)
        /// </summary>
        Notification,
    }
}