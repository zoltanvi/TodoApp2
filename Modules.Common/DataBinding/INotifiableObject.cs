namespace Modules.Common.DataBinding;

/// <summary>
/// Provides a way to bind viewmodel action to controls that can be invoked from the controls.
/// </summary>
public interface INotifiableObject
{
    /// <summary>
    /// The notification callback that is invoked from the control.
    /// </summary>
    void Notify();
}
