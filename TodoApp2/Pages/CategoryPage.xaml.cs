using TodoApp2.Core;

namespace TodoApp2
{
    public partial class CategoryPage : BasePage<CategoryPageViewModel>
    {
        public CategoryPage(CategoryPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}