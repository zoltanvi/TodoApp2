using Modules.Common.Services.Navigation;
using System.Windows.Controls;

namespace Modules.Common.Views.Services.Navigation;

public class OverlayPageNavigationService : NavigationService, IOverlayPageNavigationService
{
    public OverlayPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
