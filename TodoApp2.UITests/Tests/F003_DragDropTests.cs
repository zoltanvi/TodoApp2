using System.Collections.Generic;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    [TestFixture, Order(3)]
    public class F003_DragDropTests : TestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void T0010_AddFiveItems()
        {
            Logger.Info($"Commencing test {nameof(T0010_AddFiveItems)}");
            AddListItemsAndAssert(5);
        }

        [Test]
        public void T0030_DragDrop_NormalSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0030_DragDrop_NormalSpeed)}");
            AssertListItems();
            DragDropTaskListItem(3, 1, Constants.NormalMouseSpeed);
            DragDropTaskListItem(1, 4, Constants.NormalMouseSpeed);
            DragDropTaskListItem(2, 4, Constants.NormalMouseSpeed);
            DragDropTaskListItem(0, 2, Constants.NormalMouseSpeed);
            DragDropTaskListItem(1, 3, Constants.NormalMouseSpeed);
            DragDropTaskListItem(4, 0, Constants.NormalMouseSpeed);
            DragDropTaskListItem(1, 1, Constants.NormalMouseSpeed);
        }

        [Test]
        public void T0040_DragDrop_FastSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0040_DragDrop_FastSpeed)}");
            AssertListItems();
            DragDropTaskListItem(0, 1, Constants.FastMouseSpeed);
            DragDropTaskListItem(1, 2, Constants.FastMouseSpeed);
            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(0, 4, Constants.FastMouseSpeed);
            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(0, 1, Constants.FastMouseSpeed);
            DragDropTaskListItem(1, 1, Constants.FastMouseSpeed);
            DragDropTaskListItem(1, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(3, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(4, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(2, 0, Constants.FastMouseSpeed);
            DragDropTaskListItem(3, 0, Constants.FastMouseSpeed);
        }

        [Test]
        public void T0050_DragDrop_VeryFastSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0050_DragDrop_VeryFastSpeed)}");
            AssertListItems();
            DragDropTaskListItem(1, 3, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(0, 2, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(1, 4, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(4, 2, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(4, 2, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(3, 0, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(3, 4, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(3, 3, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(2, 1, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(2, 4, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(0, 1, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(2, 1, Constants.VeryFastMouseSpeed);
        }

        [Test]
        public void T0060_DeleteThreeItems()
        {
            Logger.Info($"Commencing test {nameof(T0060_DeleteThreeItems)}");
            List<int> indexes = new List<int> { 0, 1, 0, 0, 0};
            DeleteListItemsAndAssert(indexes);
        }

        [Test]
        public void T0070_DragDrop_VeryFastSpeed()
        {
            Logger.Info($"Commencing test {nameof(T0070_DragDrop_VeryFastSpeed)}");
            
            AddListItemsAndAssert(3);
            DragDropTaskListItem(0, 2, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(0, 2, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(1, 0, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(0, 1, Constants.VeryFastMouseSpeed);
            DragDropTaskListItem(2, 0, Constants.VeryFastMouseSpeed);
            Wait.For(Constants.OneSec);
        }

        [Test]
        public void T0080_DeleteThreeItems()
        {
            Logger.Info($"Commencing test {nameof(T0080_DeleteThreeItems)}");
            List<int> indexes = new List<int> { 1, 0, 0};
            DeleteListItemsAndAssert(indexes);
        }

        private void DragDropTaskListItem(int from, int to, int mouseSpeed)
        {
            Logger.Info($"Drag & dropping from [{from}] to [{to}]");
            ExpectedTaskList?.Move(from, to);
            var sourceItem = TaskListView.Children[from].Bounds.Center();
            var destinationItem = TaskListView.Children[to].Bounds.Center();

            // Add a little offset because the source and destination can be the same
            destinationItem.X += 20;
            destinationItem.Y += (from < to) ? 8 : -8;
            Mouse.Drag(MouseButton.Left, sourceItem, destinationItem, mouseSpeed);
            AssertListItems();
        }
    }
}