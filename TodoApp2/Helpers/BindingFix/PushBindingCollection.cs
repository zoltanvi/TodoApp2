using System.Collections.Specialized;
using System.Windows;
using PropertyChanged;

namespace TodoApp2;

/// <summary>
/// Helper class to solve a WPF bug: bind to a read-only dependency property
/// </summary>
[DoNotNotify]
public class PushBindingCollection : FreezableCollection<PushBinding>
{
    public PushBindingCollection() { }

    public PushBindingCollection(DependencyObject targetObject)
    {
        TargetObject = targetObject;
        ((INotifyCollectionChanged)this).CollectionChanged += CollectionChanged;
    }

    void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (PushBinding pushBinding in e.NewItems)
            {
                pushBinding.SetupTargetBinding(TargetObject);
            }
        }
    }

    public DependencyObject TargetObject { get; private set; }
}
