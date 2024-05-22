using System.Windows;
using TodoApp2.Core;

namespace TodoApp2;

/// <summary>
/// Interaction logic for ErrorWindow.xaml
/// </summary>
public partial class ErrorWindow : Window
{
    private ErrorWindowViewModel _viewModel;

    public ErrorWindow(string title, string errorMessage)
    {
        InitializeComponent();
        _viewModel = new ErrorWindowViewModel(title, errorMessage);
        DataContext = _viewModel;
    }

    public void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class ErrorWindowViewModel : BaseViewModel
{
    public ErrorWindowViewModel(string title, string errorMessage)
    {
        Title = title;
        ErrorMessage = errorMessage;
    }

    public string Title { get; set; }
    public string ErrorMessage { get; set; }
}
