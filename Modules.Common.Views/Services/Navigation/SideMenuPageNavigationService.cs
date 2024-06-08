using Modules.Common.Services.Navigation;
using System.Windows.Controls;

namespace Modules.Common.Views.Services.Navigation;

public class SideMenuPageNavigationService : NavigationService, ISideMenuPageNavigationService
{
    public SideMenuPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
