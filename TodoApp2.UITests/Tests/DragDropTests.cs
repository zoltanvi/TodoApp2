using System.Collections.Generic;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    public class DragDropTests : TestBase
    {
        private ListView m_TaskListView;
        private TextBox m_AddTaskTextBox;
        private List<string> m_ExpectedTaskList;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_TaskListView = Window.FindListView(UINames.TaskListListView);
            m_AddTaskTextBox = Window.FindTextBox(UINames.AddNewTaskTextBox);
        }

        [Test]
        public void T0010_AddFiveItems()
        {
            Logger.Info($"Commencing test {nameof(T0010_AddFiveItems)}");

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
        public void T0020_DragDrop_SlowSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0020_DragDrop_SlowSpeed)}");
            AssertListItems();

            DragDropTaskListItem(3, 1, Constants.SlowMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 4, Constants.SlowMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 4, Constants.SlowMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T0030_DragDrop_NormalSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0030_DragDrop_NormalSpeed)}");
            AssertListItems();

            // Before: 000, 001, 002, 003, 004
            // After:  001, 002, 000, 003, 004
            DragDropTaskListItem(0, 2, Constants.NormalMouseSpeed);
            AssertListItems();

            // Before:  001, 002, 000, 003, 004
            // After:   001, 000, 003, 002, 004
            DragDropTaskListItem(1, 3, Constants.NormalMouseSpeed);
            AssertListItems();

            // Before:  001, 000, 003, 002, 004
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(4, 0, Constants.NormalMouseSpeed);
            AssertListItems();

            // Before:  004, 001, 000, 003, 002
            // After:   004, 001, 000, 003, 002
            DragDropTaskListItem(1, 1, Constants.NormalMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T0040_DragDrop_FastSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0040_DragDrop_FastSpeed)}");

            AssertListItems();

            DragDropTaskListItem(0, 1, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 2, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 4, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 1, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 1, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 0, Constants.FastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, Constants.FastMouseSpeed);
            AssertListItems();
        }

        [Test]
        public void T0050_DragDrop_VeryFastSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0050_DragDrop_VeryFastSpeed)}");

            AssertListItems();

            DragDropTaskListItem(1, 3, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 2, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(1, 4, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 2, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(4, 2, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 0, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 4, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(3, 3, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 1, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 4, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(0, 1, Constants.VeryFastMouseSpeed);
            AssertListItems();

            DragDropTaskListItem(2, 1, Constants.VeryFastMouseSpeed);
            AssertListItems();
        }

        #region Helpers

        private void AssertListItems()
        {
            List<string> actualList = new List<string>();

            for (int i = 0; i < m_ExpectedTaskList.Count; i++)
            {
                TextBlock textBlock = m_TaskListView.Children[i].FindTextBlock("TaskListItemDisplayText");
                actualList.Add(textBlock.Text);
            }

            Logger.Info($"Expected list: {string.Join(", ", m_ExpectedTaskList)}");
            Logger.Info($"Actual list:   {string.Join(", ", actualList)}");

            CollectionAssert.AreEqual(m_ExpectedTaskList, actualList);

            Logger.Info("Lists are equal.");
        }

        private void DragDropTaskListItem(int from, int to, int mouseSpeed)
        {
            Logger.Info($"Drag & dropping from [{from}] to [{to}]");
            m_ExpectedTaskList?.Move(from, to);
            var fromItem = m_TaskListView.Children[from].Bounds.Center();
            var toItem = m_TaskListView.Children[to].Bounds.Center();
            Mouse.Drag(MouseButton.Left, fromItem, toItem, mouseSpeed);
        }

        #endregion Helpers
    }
}