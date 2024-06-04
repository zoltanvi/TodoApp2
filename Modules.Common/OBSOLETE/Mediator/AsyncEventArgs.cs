namespace Modules.Common.OBSOLETE.Mediator;

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
