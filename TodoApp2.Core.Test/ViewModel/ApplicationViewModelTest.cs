using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace TodoApp2.Core.Test.ViewModel
{
    [TestFixture]
    public class ApplicationViewModelTest
    {
        private ApplicationViewModel m_ApplicationViewModel;
        private IBaseViewModel m_BaseViewModelMock;

        [SetUp]
        public void Setup()
        {
            m_ApplicationViewModel = new ApplicationViewModel();
            m_BaseViewModelMock = Substitute.For<IBaseViewModel>();
        }

        private static IEnumerable<TestCaseData> OverlayPageTestData
        {
            get
            {
                yield return new TestCaseData(ApplicationPage.Notification);
                yield return new TestCaseData(ApplicationPage.Reminder);
                yield return new TestCaseData(ApplicationPage.Settings);
            }
        }

        [TestCaseSource(nameof(OverlayPageTestData))]
        public void GoToOverlayPage_PageChanged_Test(ApplicationPage page)
        {
            // Act
            m_ApplicationViewModel.GoToOverlayPage(page, true, m_BaseViewModelMock);

            // Assert
            Assert.AreEqual(page, m_ApplicationViewModel.OverlayPage);
        }

        [TestCaseSource(nameof(OverlayPageTestData))]
        public void GoToOverlayPage_SideMenuClosed_Test(ApplicationPage page)
        {
            // Act
            m_ApplicationViewModel.GoToOverlayPage(page, true, m_BaseViewModelMock);

            // Assert
            Assert.AreEqual(false, m_ApplicationViewModel.SideMenuVisible);
        }

        [TestCaseSource(nameof(OverlayPageTestData))]
        public void GoToOverlayPage_ViewModelSet_Test(ApplicationPage page)
        {
            // Act
            m_ApplicationViewModel.GoToOverlayPage(page, true, m_BaseViewModelMock);

            // Assert
            Assert.AreEqual(m_BaseViewModelMock, m_ApplicationViewModel.OverlayPageViewModel);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void GoToOverlayPage_PageVisible_Test(bool visible)
        {
            // Act
            m_ApplicationViewModel.GoToOverlayPage(ApplicationPage.Notification, visible, m_BaseViewModelMock);

            // Assert
            Assert.AreEqual(visible, m_ApplicationViewModel.OverlayPageVisible);
        }

        [TestCaseSource(nameof(OverlayPageTestData))]
        public void GoToOverlayPage_PageNotChanged_PropertyChangeInvoked_Test(ApplicationPage page)
        {
            // Arrange
            bool isPropertyChanged = false;
            // Set the overlay page to the expected one
            m_ApplicationViewModel.GoToOverlayPage(page, true, m_BaseViewModelMock);
            // Listen for property change event from now on
            m_ApplicationViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ApplicationViewModel.OverlayPage))
                {
                    isPropertyChanged = true;
                }
            };

            // Act
            m_ApplicationViewModel.GoToOverlayPage(page, true, m_BaseViewModelMock);

            // Assert
            Assert.AreEqual(true, isPropertyChanged);
        }
    }
}