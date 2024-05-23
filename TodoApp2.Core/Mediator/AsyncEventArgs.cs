using System;
using System.Threading.Tasks;

namespace TodoApp2.Core;

public class AsyncEventArgs : EventArgs
{
    public Func<Task> Func;
    public object Parameter;

    public AsyncEventArgs(Func<Task> func, object parameter)
    {
        Func = func;
        Parameter = parameter;
    }
}
