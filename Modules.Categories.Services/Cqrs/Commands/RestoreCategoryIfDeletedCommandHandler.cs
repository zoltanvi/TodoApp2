using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RestoreCategoryIfDeletedCommandHandler : IRequestHandler<RestoreCategoryIfDeletedCommand>
{
    public Task Handle(RestoreCategoryIfDeletedCommand request, CancellationToken cancellationToken)
    {
        // TODO: implement
        return Task.CompletedTask;
    }
}
