using System;

namespace TodoApp2.Core
{
    public class OneEditorOpenService : BaseViewModel
    {
        private TaskViewModel _editorOpenTask;

        public Action OnEditModeEnd { get; set; }
        public int LastEditedTaskId { get; set; }

        public void DisplayMode(TaskViewModel taskListItemViewModel)
        {
            if (_editorOpenTask == taskListItemViewModel)
            {
                _editorOpenTask = null;
                OnEditModeEnd?.Invoke();
            }
        }

        public void EditMode(TaskViewModel taskListItemViewModel)
        {
            // Save changes and close editor for old task
            if (_editorOpenTask != null && _editorOpenTask != taskListItemViewModel)
            {
                _editorOpenTask.UpdateContent();
            }

            _editorOpenTask = taskListItemViewModel;
            
            if (_editorOpenTask != null)
            {
                LastEditedTaskId = taskListItemViewModel.Id;
            }
        }
    }
}
