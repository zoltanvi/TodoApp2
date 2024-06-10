using MediatR;
using Modules.Categories.Contracts;
using Modules.Categories.Contracts.Cqrs.Queries;
using Modules.Categories.Contracts.Models;
using Modules.Settings.Contracts.ViewModels;

namespace Modules.Categories.Services.Cqrs.QueryHandlers;

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