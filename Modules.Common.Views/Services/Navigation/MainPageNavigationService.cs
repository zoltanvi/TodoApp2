using Modules.Common.Services.Navigation;
using System.Windows.Controls;

namespace Modules.Common.Views.Services.Navigation;

public class MainPageNavigationService : NavigationService, IMainPageNavigationService
{
    public MainPageNavigationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    //private Dictionary<Type, Page> _cache = new Dictionary<Type, Page>();

    //protected override Page? GetPage<T>()
    //{
    //    if (_cache.ContainsKey(typeof(T)))
    //    {
    //        return _cache[typeof(T)];
    //    }

    //    var page = GetPageFromServices<T>();
    //    if ((page == null))
    //    {
    //        throw new InvalidOperationException("Cannot get page from services.");
    //    }

    //    _cache.Add(typeof(T), page);
    //    return page;
    //}
}
