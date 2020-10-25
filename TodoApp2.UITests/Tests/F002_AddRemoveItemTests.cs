using System.Collections.Generic;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    [TestFixture, Order(2)]
    public class F002_AddRemoveItemTests : TestBase
    {
        private ListView m_TaskListView;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_TaskListView = Window.FindListView(UINames.TaskListListView);
        }

        [Test]
        public void T0010_AddTwoItems()
        {
            Logger.Info($"Commencing test {nameof(T0010_AddTwoItems)}");
            AddListItemsAndAssert(2);
        }

        [Test]
        public void T0020_DeleteTwoItems()
        {
            Logger.Info($"Commencing test {nameof(T0020_DeleteTwoItems)}");
            // Delete the first item 2 times
            List<int> indexes = new List<int> { 0, 0 };
            DeleteListItemsAndAssert(indexes);
        }

        [Test]
        public void T0030_AddTenItems()
        {
            Logger.Info($"Commencing test {nameof(T0030_AddTenItems)}");
            AddListItemsAndAssert(10);
        }

        [Test]
        public void T0040_DeleteTenItemsRandomOrder()
        {
            Logger.Info($"Commencing test {nameof(T0040_DeleteTenItemsRandomOrder)}");
            // Delete the items in random order
            List<int> indexes = new List<int> { 7, 4, 3, 5, 0, 3, 1, 2, 0, 0 };
            DeleteListItemsAndAssert(indexes);
        }
    }
}