using System.Windows.Input;

namespace TodoApp2.Core
{
    /// <summary>
    /// The View Model for the task page
    /// </summary>
    public class TaskViewModel : BaseViewModel
    {
        #region Private Fields


        #endregion

        #region Commands

        public ICommand MyCommand { get; set; }

        #endregion

        #region Public Properties


        #endregion

        #region Constructors

        public TaskViewModel()
        {
            // Create commands
            MyCommand = new RelayCommand(() => { });


        }

        #endregion

        #region EventHandlers



        #endregion

    }
}
