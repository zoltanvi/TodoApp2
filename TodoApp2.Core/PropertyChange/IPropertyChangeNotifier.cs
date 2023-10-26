using PropertyChanged;
using System.ComponentModel;

namespace TodoApp2.Core
{
    /// <summary>
    /// Provides a method to trigger a property changed event.
    /// </summary>
    public interface IPropertyChangeNotifier : INotifyPropertyChanged
    {
        void OnPropertyChanged(string name);
    }
}
