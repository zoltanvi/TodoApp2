using Gu.Wpf.UiAutomation;
using NUnit.Framework;

namespace TodoApp2.UITests
{
    public class Tests
    {
        private const string ExeFileName = "TodoApp2.exe";

        [SetUp]
        public void Setup()
        {
            using var app = Application.AttachOrLaunch(ExeFileName);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
        }

        [Test]
        public void Test1()
        {
            using var app = Application.AttachOrLaunch(ExeFileName);
            var window = app.MainWindow;

            window.FindButton("MaximizeWindow").Invoke();
            Wait.For(new System.TimeSpan(0, 0, 2));
            window.FindButton("MinimizeWindow").Invoke();
        }
    }
}