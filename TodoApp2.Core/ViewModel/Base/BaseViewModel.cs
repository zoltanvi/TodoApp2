using PropertyChanged;
using System.ComponentModel;

namespace TodoApp2.Core;

/// <summary>
/// A base view model that fires Property Changed events as needed
/// </summary>
[AddINotifyPropertyChangedInterface]
public abstract class BaseViewModel : IBaseViewModel
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

    public void Dispose()
    {
        OnDispose();
    }

    protected virtual void OnDispose()
    {
    }
}