using Modules.Common.Navigation;

namespace Modules.Common.Services.Navigation;

public interface INavigationService
{
    void NavigateTo<T>() where T : class, IPage;
    void Initialize(object frame);
}