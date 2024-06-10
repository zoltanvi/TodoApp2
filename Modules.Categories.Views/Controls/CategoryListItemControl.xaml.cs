using System.Windows;
using System.Windows.Controls;

namespace Modules.Categories.Views.Controls;

/// <summary>
/// Interaction logic for CategoryListItemControl.xaml
/// </summary>
public partial class CategoryListItemControl : UserControl
{
    public static readonly DependencyProperty ActiveCategoryIdProperty =
       DependencyProperty.Register(nameof(ActiveCategoryId), typeof(int), typeof(CategoryListItemControl), new PropertyMetadata(-1));

    public int ActiveCategoryId
    {
        get => (int)GetValue(ActiveCategoryIdProperty);
        set => SetValue(ActiveCategoryIdProperty, value);
    }

    public CategoryListItemControl()
    {
        InitializeComponent();
    }
}