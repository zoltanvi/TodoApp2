using Modules.Common.DataBinding;

namespace Modules.Common.ViewModel;

/// <summary>
/// Marker interface for view models.
/// See: <see cref="IPropertyChangeNotifier"/>.
/// </summary>
public interface IBaseViewModel : IPropertyChangeNotifier, IDisposable
{
}