using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RestoreCategoryIfDeletedCommand : IRequest
{
    public int Id { get; set; }
}
