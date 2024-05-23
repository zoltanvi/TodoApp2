using PropertyChanged;
using System.ComponentModel;

namespace Modules.Common.DataBinding;

/// <summary>
/// Provides a method to trigger a property changed event.
/// </summary>
public interface IPropertyChangeNotifier : INotifyPropertyChanged
{
    void OnPropertyChanged(string name);
}
