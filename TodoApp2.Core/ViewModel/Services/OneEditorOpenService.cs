namespace TodoApp2.Core
{
    public class OneEditorOpenService : BaseViewModel
    {
        private TaskListItemViewModel m_EditorOpenTask;

        internal void DisplayMode(TaskListItemViewModel taskListItemViewModel)
        {
            if (m_EditorOpenTask == taskListItemViewModel)
            {
                m_EditorOpenTask = null;
            }
        }

        internal void EditMode(TaskListItemViewModel taskListItemViewModel)
        {
            // Save changes and close editor for old task
            if (m_EditorOpenTask != null && m_EditorOpenTask != taskListItemViewModel)
            {
                m_EditorOpenTask.UpdateContent();
            }

            m_EditorOpenTask = taskListItemViewModel;
        }
    }
}
