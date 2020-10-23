using System.Windows.Automation;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    public class TestBase
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Window Window;
        public AutomationElement WindowAutomationElement;
        public Button WindowMinimizeButton;
        public Button WindowMaximizeButton;
        public Button WindowCloseButton;

        [OneTimeSetUp]
        public void InitFixture()
        {
            using var app = Application.AttachOrLaunch(Constants.ExeFileName);

            Window = app.MainWindow;
            WindowMinimizeButton = Window.FindButton(UINames.MinimizeWindowButton);
            WindowMaximizeButton = Window.FindButton(UINames.MaximizeWindowButton);
            WindowCloseButton = Window.FindButton(UINames.CloseWindowButton);

            WindowAutomationElement = AutomationElement.RootElement?.FindFirst(TreeScope.Children, Conditions.ByAutomationId(UINames.Window));
        }
    }
}
