using MediatR;
using Modules.Categories.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Contracts.Cqrs.Queries;

public class GetActiveCategoryInfoQueryHandler : IRequestHandler<GetActiveCategoryInfoQuery, ActiveCategoryInfo>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public GetActiveCategoryInfoQueryHandler(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public Task<ActiveCategoryInfo> Handle(GetActiveCategoryInfoQuery request, CancellationToken cancellationToken)
    {
        var id = AppSettings.Instance.SessionSettings.ActiveCategoryId;
        Category? activeCategory = _categoriesRepository.GetCategoryById(id);

        return Task.FromResult(new ActiveCategoryInfo()
        { 
            Id = activeCategory?.Id ?? -1,
            Name = activeCategory?.Name ?? string.Empty
        });
    }
}