using System;
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

        private static readonly TimeSpan HalfSec = new TimeSpan(0, 0, 0, 500);
        private static readonly TimeSpan OneSec = new TimeSpan(0, 0, 1);
        private static readonly TimeSpan TwoSec = new TimeSpan(0, 0, 2);
        private static readonly TimeSpan ThreeSec = new TimeSpan(0, 0, 3);

        private const int VeryFastMouseSpeed = 2000;
        private const int FastMouseSpeed = 600;
        private const int NormalMouseSpeed = 300;
        private const int SlowMouseSpeed = 150;

        private const string TaskListListViewName = "TaskListListView";
        private const string AddNewTaskTextBoxName = "AddNewTaskTextBox";

        private Window m_Window;
        private ListView m_TaskListView;
        private TextBox m_AddTaskTextBox;
        private List<string> m_ExpectedTaskList;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Delete database file
            File.Delete(DatabasePath);
            Wait.For(OneSec);

            using var app = Application.AttachOrLaunch(ExeFileName);
            m_Window = app.MainWindow;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Application.KillLaunched(ExeFileName);
        }

        [SetUp]
        public void SetUp()
        {
            m_TaskListView = m_Window.FindListView(TaskListListViewName);
            m_AddTaskTextBox = m_Window.FindTextBox(AddNewTaskTextBoxName);
        }

        [Test]
        public void T001_AddFiveItems()
        {
            // Add items in reverse order, so the task list looks like this:
            // 000, 001, 002, 003, 004
            m_ExpectedTaskList = new List<string>();
            for (int i = 4; i >= 0; i--)
            {
                string text = $"00{i}";
                m_ExpectedTaskList.Add(text);
                m_AddTaskTextBox.Enter($"{text}\n");
            }
            m_ExpectedTaskList.Reverse();

            AssertListItems();
        }

        [Test]
        public void T002_DragDrop_SlowSpeed()
        {
            AssertListItems();

            DragDropTaskListItem(3, 1, SlowMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 4, SlowMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 4, SlowMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T003_DragDrop_NormalSpeed()
        {
            AssertListItems();

            // Before: 000, 001, 002, 003, 004
            // After:  001, 002, 000, 003, 004
            DragDropTaskListItem(0, 2, NormalMouseSpeed);
            AssertListItems();

            // Before:  001, 002, 000, 003, 004
            // After:   001, 000, 003, 002, 004
            DragDropTaskListItem(1, 3, NormalMouseSpeed);
            AssertListItems();

            // Before:  001, 000, 003, 002, 004
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(4, 0, NormalMouseSpeed);
            AssertListItems();

            // Before:  004, 001, 000, 003, 002
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(1, 1, NormalMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T004_DragDrop_FastSpeed()
        {
            AssertListItems();

            DragDropTaskListItem(0, 1, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 2, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 4, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 1, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 1, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 0, FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, FastMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T005_DragDrop_VeryFastSpeed()
        {
            AssertListItems();

            DragDropTaskListItem(1, 3, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 2, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 4, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 2, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 2, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 4, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 3, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 1, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 4, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 1, VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 1, VeryFastMouseSpeed);
            AssertListItems();
        }

        #region Helpers

        private void AssertListItems()
        {
            for (int i = 0; i < m_ExpectedTaskList.Count; i++)
            {
                var textBlock = m_TaskListView.Children[i].FindTextBlock();
                Assert.AreEqual(m_ExpectedTaskList[i], textBlock.Text);
            }
        }

        private void DragDropTaskListItem(int from, int to, int mouseSpeed)
        {
            m_ExpectedTaskList?.Move(from, to);
            var fromItem = m_TaskListView.Children[from].Bounds.Center();
            var toItem = m_TaskListView.Children[to].Bounds.Center();
            Mouse.Drag(MouseButton.Left, fromItem, toItem, mouseSpeed);
        }

        #endregion Helpers
    }
}