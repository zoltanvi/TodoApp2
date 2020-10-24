using System.Collections.Generic;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;

namespace TodoApp2.UITests.Tests
{
    [TestFixture, Order(1)]
    public class F001_AddRemoveItemTests : TestBase
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

            for (int i = 1; i >= 0; i--)
            {
                Logger.Info($"Deleting first item");
                Button deleteButton = TaskListView.Children[0].FindButton(UINames.TaskListItemTrashBinButton);

                MoveMouseAndClick(deleteButton);

                ExpectedTaskList.RemoveAt(0);
                
                AssertListItems();
            }
        }

    }
}
