using System;

namespace TodoApp2.Core;

public class ZoomedEventArgs : EventArgs
{
    public double OldScaleValue { get; set; }
    
    public double NewScaleValue { get; set; }

    public bool Shrinked => OldScaleValue > NewScaleValue;

    public ZoomedEventArgs(double oldScaleValue, double newScaleValue)
    {
        OldScaleValue = oldScaleValue;
        NewScaleValue = newScaleValue;
    }
}
