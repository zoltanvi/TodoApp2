using System;
using System.Windows;

namespace TodoApp2.WindowHandling;
public interface IWindowService
{
    double Height { get; set; }
    bool IsDocked { get; }
    bool IsMaximized { get; }
    bool IsMaximizedOrDocked { get; }
    bool IsMinimized { get; }
    bool IsRoundedCornersAllowed { get; }
    double Left { get; set; }
    double Top { get; set; }
    bool Topmost { get; set; }
    double Width { get; set; }
    WindowState WindowState { get; set; }

    event EventHandler Closed;
    event EventHandler Closing;
    event EventHandler Deactivated;
    event EventHandler Loaded;
    event EventHandler Resized;
    event EventHandler RoundedCornersChanged;
    event EventHandler StateChanged;

    void Activate();
    void Close();
    void FlashWindow(bool topmost);
    void Focus();
    void Hide();
    void Maximize();
    void Minimize();
    void SetPosition(double left, double top);
    void SetSize(double width, double height);
    void Show();
}