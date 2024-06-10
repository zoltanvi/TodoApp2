using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RenameActiveCategoryCommand : IRequest
{
    public string Name { get; set; }
}
