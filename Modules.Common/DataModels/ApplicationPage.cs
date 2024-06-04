namespace Modules.Common.DataModels;

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
    /// The recycle bin page
    /// </summary>
    RecycleBin,

    /// <summary>
    /// The task reminder page (where the reminder(s) can be added / removed / activated / deactivated)
    /// </summary>
    TaskReminder,

    /// <summary>
    /// The reminder page (where a single reminder can be edited)
    /// </summary>
    ReminderEditor,

    /// <summary>
    /// The notification page (pops up when a notification occurs)
    /// </summary>
    Notification,

    /// <summary>
    /// The note page
    /// </summary>
    Note,

    /// <summary>
    /// The note list page (side menu)
    /// </summary>
    NoteList,
}