using Modules.Common.Navigation;
using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    public partial class CategoryPage : GenericBasePage<CategoryPageViewModel>, ICategoryListPage
    {
        public CategoryPage(CategoryPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}