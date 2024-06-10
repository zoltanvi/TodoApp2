using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Queries;

public class GetActiveCategoryInfoQuery : IRequest<ActiveCategoryInfo>
{
}

public class ActiveCategoryInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
}
