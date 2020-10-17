using System.ComponentModel;

namespace TodoApp2.Core
{
    public interface IBaseViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name);
    }
}