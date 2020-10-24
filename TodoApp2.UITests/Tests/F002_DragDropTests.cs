using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    [TestFixture, Order(2)]
    public class F002_DragDropTests : TestBase
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

        private void DragDropTaskListItem(int from, int to, int mouseSpeed)
        {
            Logger.Info($"Drag & dropping from [{from}] to [{to}]");
            ExpectedTaskList?.Move(from, to);
            var sourceItem = TaskListView.Children[from].Bounds.Center();
            var destinationItem = TaskListView.Children[to].Bounds.Center();
            
            // Add a little offset because the source and destination can be the same
            destinationItem.X += 5;
            destinationItem.Y += 3;
            Mouse.Drag(MouseButton.Left, sourceItem, destinationItem, mouseSpeed);
        }
    }
}