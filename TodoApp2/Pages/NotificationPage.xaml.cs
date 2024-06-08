using Modules.Common.Navigation;
using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for NotificationPage.xaml
    /// </summary>
    public partial class NotificationPage : GenericBasePage<NotificationPageViewModel>, ITaskNotificationPage
    {
        public NotificationPage(NotificationPageViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }
    }
}