using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Commands;

public class RenameActiveCategoryCommandHandler : IRequestHandler<RenameActiveCategoryCommand>
{
    public Task Handle(RenameActiveCategoryCommand request, CancellationToken cancellationToken)
    {
        // TODO: implement
        return Task.CompletedTask;
    }
}
