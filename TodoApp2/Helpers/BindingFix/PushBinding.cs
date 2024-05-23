using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace TodoApp2;

/// <summary>
/// Helper class to solve a WPF bug: bind to a read-only dependency property
/// </summary>
public class PushBinding : FreezableBinding
{
    public static DependencyProperty TargetPropertyMirrorProperty = DependencyProperty.Register(nameof(TargetPropertyMirror), typeof(object), typeof(PushBinding));
    public static DependencyProperty TargetPropertyListenerProperty = DependencyProperty.Register(nameof(TargetPropertyListener), typeof(object), typeof(PushBinding), new UIPropertyMetadata(null, OnTargetPropertyListenerChanged));

    private static void OnTargetPropertyListenerChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        PushBinding pushBinding = sender as PushBinding;
        pushBinding.TargetPropertyValueChanged();
    }

    public PushBinding()
    {
        Mode = BindingMode.OneWayToSource;
    }

    public object TargetPropertyMirror
    {
        get => GetValue(TargetPropertyMirrorProperty);
        set => SetValue(TargetPropertyMirrorProperty, value);
    }
    public object TargetPropertyListener
    {
        get => GetValue(TargetPropertyListenerProperty);
        set => SetValue(TargetPropertyListenerProperty, value);
    }

    [DefaultValue(null)]
    public string TargetProperty { get; set; }

    [DefaultValue(null)]
    public DependencyProperty TargetDependencyProperty { get; set; }

    public void SetupTargetBinding(DependencyObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        // Prevent the designer from reporting exceptions since
        // changes will be made of a Binding in use if it is set
        if (DesignerProperties.GetIsInDesignMode(this) == true)
            return;

        // Bind to the selected TargetProperty, e.g. ActualHeight and get
        // notified about changes in OnTargetPropertyListenerChanged
        Binding listenerBinding = new Binding
        {
            Source = targetObject,
            Mode = BindingMode.OneWay
        };
        if (TargetDependencyProperty != null)
        {
            listenerBinding.Path = new PropertyPath(TargetDependencyProperty);
        }
        else
        {
            listenerBinding.Path = new PropertyPath(TargetProperty);
        }
        BindingOperations.SetBinding(this, TargetPropertyListenerProperty, listenerBinding);

        // Set up a OneWayToSource Binding with the Binding declared in Xaml from
        // the Mirror property of this class. The mirror property will be updated
        // everytime the Listener property gets updated
        BindingOperations.SetBinding(this, TargetPropertyMirrorProperty, Binding);

        TargetPropertyValueChanged();
        if (targetObject is FrameworkElement)
        {
            ((FrameworkElement)targetObject).Loaded += delegate { TargetPropertyValueChanged(); };
        }
        else if (targetObject is FrameworkContentElement)
        {
            ((FrameworkContentElement)targetObject).Loaded += delegate { TargetPropertyValueChanged(); };
        }
    }

    private void TargetPropertyValueChanged()
    {
        object targetPropertyValue = GetValue(TargetPropertyListenerProperty);
        this.SetValue(TargetPropertyMirrorProperty, targetPropertyValue);
    }

    protected override void CloneCore(Freezable sourceFreezable)
    {
        PushBinding pushBinding = sourceFreezable as PushBinding;
        TargetProperty = pushBinding.TargetProperty;
        TargetDependencyProperty = pushBinding.TargetDependencyProperty;
        base.CloneCore(sourceFreezable);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new PushBinding();
    }
}
