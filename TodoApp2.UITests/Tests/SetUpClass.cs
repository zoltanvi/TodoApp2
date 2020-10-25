using System.IO;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    [SetUpFixture]
    public class SetUpClass
    {
        private Button WindowCloseButton;
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Logger.Info("Setting up test fixture.");

            // Delete database file
            Logger.Info("Deleting database file");
            File.Delete(Constants.DatabasePath);

            Logger.Info("Launching application...");
            using var app = Application.AttachOrLaunch(Constants.ExeFileName);
            WindowCloseButton = app.MainWindow.FindButton(UINames.CloseWindowButton);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            Logger.Info("Cleaning up test fixture.");
            Logger.Info("Shutting down application...");
            LogManager.Shutdown();
            WindowCloseButton.Invoke();
        }
    }
}