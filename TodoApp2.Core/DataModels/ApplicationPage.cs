namespace TodoApp2.Core
{
    /// <summary>
    /// A page of the application
    /// </summary>
    public enum ApplicationPage
    {
        /// <summary>
        /// The task page
        /// </summary>
        Task,

        /// <summary>
        /// The side menu page
        /// </summary>
        SideMenu,

        /// <summary>
        /// The options page (opened by cog in the side menu page)
        /// </summary>
        Options,

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