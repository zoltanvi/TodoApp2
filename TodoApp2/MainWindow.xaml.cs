using System;
using System.Windows;

namespace TodoApp2;

public partial class MainWindow : Window
{
    private GridResizer _gridResizer;

    public MainWindow()
    {
        InitializeComponent();

        _gridResizer = new GridResizer(Grid, Resizer, this);
    }
}
