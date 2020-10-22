using System;
using System.Windows.Automation;
using NUnit.Framework;

namespace TodoApp2.UITests
{

    // TODO: Make it work :)
    public class SideMenuTests : TestBase
    {
        [Test]
        public void T006_OpenCloseSideMenu()
        {
            AutomationElement rootElement = AutomationElement.RootElement;
            if (rootElement != null)
            {
                Condition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, "AppWindow");

                AutomationElement appElement = rootElement.FindFirst(TreeScope.Children, condition);

                if (appElement != null)
                {
                    AutomationElement txtElementA = appElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "SideMenuButton"));
                    if (txtElementA != null)
                    {
                        ValuePattern valuePatternA =
                          txtElementA.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        valuePatternA.SetValue("10");
                    }

                    AutomationElement txtElementB = GetTextElement(appElement, "txtB");
                    if (txtElementA != null)
                    {
                        ValuePattern valuePatternB =
                          txtElementB.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        valuePatternB.SetValue("5");
                    }
                }
            }
        }

        private AutomationElement GetTextElement(AutomationElement parentElement, string value)
        {
            Condition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, value);
            AutomationElement txtElement = parentElement.FindFirst(TreeScope.Descendants, condition);
            return txtElement;
        }

    }
}
