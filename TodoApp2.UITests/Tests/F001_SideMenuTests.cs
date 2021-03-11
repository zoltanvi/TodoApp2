﻿using System.Windows.Automation;
using Gu.Wpf.UiAutomation;
using NUnit.Framework;
using TodoApp2.UITests.Automation;
using Point = System.Windows.Point;
using Rect = System.Windows.Rect;

namespace TodoApp2.UITests.Tests
{
    [TestFixture, Order(1)]
    public class F001_SideMenuTests : TestBase
    {
        private const int s_SideMenuTreshold = 200;
        private Button m_SideMenuButton;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            m_SideMenuButton = Window.FindButton(UINames.SideMenuButton);
        }

        [Test]
        public void T0010_OpenCloseSideMenuWithButton()
        {
            const int testCount = 3;
            Rect sideMenuRectBefore;
            Rect sideMenuRectAfter;

            for (int i = 0; i < testCount; i++)
            {
                sideMenuRectBefore = GetUIElementRect(UINames.SideMenuPage);
                Logger.Info("Opening side menu");
                MoveMouseAndClick(m_SideMenuButton);
                sideMenuRectAfter = GetUIElementRect(UINames.SideMenuPage);

                // Wait for animation to finish
                Wait.For(Constants.HalfSec);
                Logger.Info($"Left before: {sideMenuRectBefore.Left}, left after: {sideMenuRectAfter.Left}");

                Assert.Less(sideMenuRectBefore.Left + s_SideMenuTreshold, sideMenuRectAfter.Left);
                Logger.Info("Side menu opened successfully.");

                sideMenuRectBefore = GetUIElementRect(UINames.SideMenuPage);
                Logger.Info("Closing side menu with button");
                MoveMouseAndClick(m_SideMenuButton);
                sideMenuRectAfter = GetUIElementRect(UINames.SideMenuPage);

                // Wait for animation to finish
                Wait.For(Constants.HalfSec);
                Logger.Info($"Left before: {sideMenuRectBefore.Left}, left after: {sideMenuRectAfter.Left}");

                Assert.Less(sideMenuRectAfter.Left, sideMenuRectBefore.Left - s_SideMenuTreshold);
                Logger.Info("Side menu closed successfully.");
            }
        }

        [Test]
        public void T0020_OpenCloseSideMenuWithBackground()
        {
            const int edgeOffset = 50;
            const int testCount = 3;

            for (int i = 0; i < testCount; i++)
            {
                Logger.Info("Opening side menu");
                MoveMouseAndClick(m_SideMenuButton);

                // Wait for animation to finish
                Wait.For(Constants.HalfSec);

                Rect sideMenuRectBefore = GetUIElementRect(UINames.SideMenuPage);
                Logger.Info("Closing side menu with overlay background");
                Point clickPosition = WindowCloseButton.Bounds.Center();
                clickPosition.Y += edgeOffset;
                MoveMouseAndClick(clickPosition);

                Rect sideMenuRectAfter = GetUIElementRect(UINames.SideMenuPage);
                Logger.Info($"Left before: {sideMenuRectBefore.Left}, left after: {sideMenuRectAfter.Left}");

                Assert.Less(sideMenuRectAfter.Left, sideMenuRectBefore.Left - s_SideMenuTreshold);
                Logger.Info("Side menu closed successfully.");

                Wait.For(Constants.TwoSec);
            }
        }

        private Rect GetUIElementRect(string automationId)
        {
            Rect uiElementRect = default;

            AutomationElement automationElement = WindowAutomationElement.FindFirst(TreeScope.Descendants, Conditions.ByAutomationId(automationId));
            if (automationElement != null)
            {
                uiElementRect = automationElement.Current.BoundingRectangle;
            }

            return uiElementRect;
        }
    }
}