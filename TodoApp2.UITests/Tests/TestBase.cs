using System.Collections.Generic;
using System.Windows.Automation;
using Gu.Wpf.UiAutomation;
using NLog;
using NUnit.Framework;
using TodoApp2.UITests.Automation;
using Point = System.Windows.Point;

namespace TodoApp2.UITests.Tests
{
    public class TestBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected Window Window;
        protected AutomationElement WindowAutomationElement;
        protected Button WindowMinimizeButton;
        protected Button WindowMaximizeButton;
        protected Button WindowCloseButton;

        protected ListView TaskListView;
        protected List<string> ExpectedTaskList;
        protected TextBox AddTaskTextBox;

        [OneTimeSetUp]
        public void InitFixture()
        {
            using var app = Application.AttachOrLaunch(Constants.ExeFileName);

            Window = app.MainWindow;
            WindowMinimizeButton = Window.FindButton(UINames.MinimizeWindowButton);
            WindowMaximizeButton = Window.FindButton(UINames.MaximizeWindowButton);
            WindowCloseButton = Window.FindButton(UINames.CloseWindowButton);
            WindowAutomationElement = AutomationElement.RootElement?.FindFirst(TreeScope.Children, Conditions.ByAutomationId(UINames.Window));

            TaskListView = Window.FindListView(UINames.TaskListListView);
            AddTaskTextBox = Window.FindTextBox(UINames.AddNewTaskTextBox);
        }

        protected void AddListItemsAndAssert(int count)
        {
            if (ExpectedTaskList == null)
            {
                ExpectedTaskList = new List<string>();
            }

            for (int i = count - 1; i >= 0; i--)
            {
                string text = $"TodoItem_00{i}";
                MoveMouseAndClick(AddTaskTextBox);
                Keyboard.Type($"{text}\n");
                ExpectedTaskList.Add(text);
            }
            ExpectedTaskList.Reverse();

            AssertListItems();
        }

        protected void DeleteListItemsAndAssert(IEnumerable<int> indexes)
        {
            foreach (var index in indexes)
            {
                Logger.Info($"Hovering item buttons at index [{index}].");

                // Move mouse to make the buttons appear. 
                // It is needed to get the bounds of the control.
                var xPosition = TaskListView.Children[index].Bounds.Right - 20;
                Mouse.MoveTo(new Point(xPosition, Mouse.Position.Y), Constants.FastMouseSpeed);

                Logger.Info($"Deleting item at index [{index}].");
                Button deleteButton = TaskListView.Children[index].FindButton(UINames.TaskListItemTrashBinButton);

                MoveMouseAndClick(deleteButton);
                Logger.Info($"Deleted item at index [{index}].");

                ExpectedTaskList.RemoveAt(index);
                AssertListItems();
            }
        }

        protected void AssertListItems()
        {
            List<string> actualList = new List<string>();

            for (int i = 0; i < TaskListView.Children.Count; i++)
            {
                TextBlock textBlock = TaskListView.Children[i].FindTextBlock(UINames.TaskListItemDisplayText);
                actualList.Add(textBlock.Text);
            }

            Logger.Info($"Expected list: {string.Join(", ", ExpectedTaskList)}");
            Logger.Info($"Actual list:   {string.Join(", ", actualList)}");

            CollectionAssert.AreEqual(ExpectedTaskList, actualList);

            Logger.Info("Lists are equal.");
        }

        protected void MoveMouseAndClick(UiElement uiElement)
        {
            Mouse.MoveTo(uiElement.Bounds.Center(), Constants.FastMouseSpeed);
            Mouse.LeftClick();
        }

        protected void MoveMouseAndClick(Point point)
        {
            Mouse.MoveTo(point, Constants.VeryFastMouseSpeed);
            Mouse.LeftClick(point);
        }
    }
}