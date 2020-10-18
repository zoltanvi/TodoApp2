using System.Collections.Generic;
using System.IO;
using Gu.Wpf.UiAutomation;
using NUnit.Framework;
using TodoApp2.UITests.Helpers;

namespace TodoApp2.UITests
{
    public class Tests
    {
        private const string ExeFileName = "TodoApp2.exe";
        private const string DatabaseName = "TodoApp2Database.db";
        private static string DatabasePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), DatabaseName);

        private const int MouseSpeed = 300;
        private const string TaskListListViewName = "TaskListListView";
        private const string AddNewTaskTextBoxName = "AddNewTaskTextBox";

        private List<string> m_ExpectedList;

        [SetUp]
        public void Setup()
        {
            // Delete database file
            File.Delete(DatabasePath);
            Wait.For(new System.TimeSpan(0, 0, 2));

            using var app = Application.AttachOrLaunch(ExeFileName);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
        }

        public void AddFiveItems()
        {
            using var app = Application.AttachOrLaunch(ExeFileName);
            var window = app.MainWindow;

            var listView = window.FindListView(TaskListListViewName);
            var addTaskTextBox = window.FindTextBox(AddNewTaskTextBoxName);

            // Add items in reverse order, so the list looks like:
            // 000, 001, 002, 003, 004
            m_ExpectedList = new List<string>();
            for (int i = 4; i >= 0; i--)
            {
                string text = $"00{i}";
                m_ExpectedList.Add(text);
                addTaskTextBox.Enter($"{text}\n");
            }
            m_ExpectedList.Reverse();
        }

        [Test]
        public void DragDropTest()
        {
            using var app = Application.AttachOrLaunch(ExeFileName);
            var window = app.MainWindow;

            var listView = window.FindListView(TaskListListViewName);
            var addTaskTextBox = window.FindTextBox(AddNewTaskTextBoxName);

            AddFiveItems();

            AssertListItems();

            // Before: 000, 001, 002, 003, 004
            // After:  001, 002, 000, 003, 004
            DragDropTaskListItem(listView, 0, 2, m_ExpectedList);

            AssertListItems();

            // Before:  001, 002, 000, 003, 004
            // After:   001, 000, 003, 002, 004
            DragDropTaskListItem(listView, 1, 3, m_ExpectedList);

            AssertListItems();

            // Before:  001, 000, 003, 002, 004
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(listView, 4, 0, m_ExpectedList);

            AssertListItems();

            // Before:  004, 001, 000, 003, 002
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(listView, 1, 1, m_ExpectedList);

            AssertListItems();

            void AssertListItems()
            {
                for (int i = 0; i < 5; i++)
                {
                    var textBlock = listView.Children[i].FindTextBlock();
                    Assert.AreEqual(m_ExpectedList[i], textBlock.Text);
                }
            }
        }

        private void DragDropTaskListItem(ListView listView, int from, int to, List<string> expectedList = null)
        {
            m_ExpectedList?.Move(from, to);
            var fromItem = listView.Children[from].Bounds.Center();
            var toItem = listView.Children[to].Bounds.Center();
            Mouse.Drag(MouseButton.Left, fromItem, toItem, MouseSpeed);
        }
    }
}