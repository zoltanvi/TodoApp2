using TodoApp2.Core;

namespace TodoApp2;

/// <summary>
/// Locates view models from the IoC for use in binding in XAML files
/// </summary>
public class VML
{
    public static VML Instance { get; } = new VML();

    public static AppViewModel ApplicationViewModel => IoC.AppViewModel;
    public static OverlayPageService OverlayPageService => IoC.OverlayPageService;
    public static TaskListService TaskListService => IoC.TaskListService;
    public static MessageService MessageService => IoC.MessageService;
}