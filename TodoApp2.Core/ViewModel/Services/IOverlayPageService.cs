using System;
using System.Windows.Input;

namespace TodoApp2.Core
{
    public interface IOverlayPageService
    {
        ICommand BackgroundClickedCommand { get; }
        bool OverlayBackgroundVisible { get; set; }

        void ClosePage();
        void CloseSideMenu();
        void OpenPage(ApplicationPage page, TaskViewModel task = null);
        void SetBackgroundClickedAction(Action action);
    }
}