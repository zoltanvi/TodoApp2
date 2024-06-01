using PropertyChanged;
using System.ComponentModel;

namespace Modules.Common.ViewModel;

/// <summary>
/// A base view model that fires Property Changed events as needed
/// </summary>
[AddINotifyPropertyChangedInterface]
public abstract class BaseViewModel : IBaseViewModel
{
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void Dispose()
    {
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }
}