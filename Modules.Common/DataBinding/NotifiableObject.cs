namespace Modules.Common.DataBinding;

public class NotifiableObject : INotifiableObject
{
    private Action _notifyAction;

    public NotifiableObject(Action notifyAction)
    {
        _notifyAction = notifyAction;
    }

    public void Notify() => _notifyAction?.Invoke();
}
