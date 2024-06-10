using MediatR;
using Modules.Categories.Contracts.Cqrs.Commands;

namespace Modules.Categories.Services.Cqrs.CommandHandlers;

public class RestoreCategoryIfDeletedCommandHandler : IRequestHandler<RestoreCategoryIfDeletedCommand>
{
    public Task Handle(RestoreCategoryIfDeletedCommand request, CancellationToken cancellationToken)
    {
        // TODO: implement
        return Task.CompletedTask;
    }
}
