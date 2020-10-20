using System.IO;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;

namespace TodoApp2.UITests
{
    public class TestBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected Window Window;

        protected Button WindowMinimizeButton;
        protected Button WindowMaximizeButton;
        protected Button WindowCloseButton;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Delete database file
            Logger.Info("Deleting database file");
            File.Delete(Constants.DatabasePath);
            Wait.For(Constants.HalfSec);

            Logger.Info("Launching application...");
            using var app = Application.AttachOrLaunch(Constants.ExeFileName);
            Window = app.MainWindow;
            WindowMinimizeButton = Window.FindButton(UINames.MinimizeWindowButton);
            WindowMaximizeButton = Window.FindButton(UINames.MaximizeWindowButton);
            WindowCloseButton = Window.FindButton(UINames.CloseWindowButton);

            OnOneTimeSetup();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OnOneTimeTearDown();

            Logger.Info("Shutting down application...");
            LogManager.Shutdown();
            WindowCloseButton.Invoke();
        }

        protected virtual void OnOneTimeSetup()
        {
        }

        protected virtual void OnOneTimeTearDown()
        {
        }
    }
}
