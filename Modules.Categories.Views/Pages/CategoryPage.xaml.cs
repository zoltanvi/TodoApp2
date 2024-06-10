using Modules.Common.Navigation;
using Modules.Common.Views.Pages;

namespace Modules.Categories.Views.Pages;

public partial class CategoryPage : GenericBasePage<CategoryPageViewModel>, ICategoryListPage
{
    public CategoryPage(CategoryPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}