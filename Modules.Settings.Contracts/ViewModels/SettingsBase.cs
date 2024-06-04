using Modules.Common.DataBinding;
using PropertyChanged;
using System.ComponentModel;

namespace Modules.Settings.Contracts.ViewModels;

[AddINotifyPropertyChangedInterface]
public abstract class SettingsBase : IPropertyChangeNotifier
{
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Call this to fire a <see cref="PropertyChanged"/> event
    /// </summary>
    /// <param name="name"></param>
    public void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
