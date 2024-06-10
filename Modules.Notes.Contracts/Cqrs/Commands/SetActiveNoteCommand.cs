using MediatR;

namespace Modules.Notes.Contracts.Cqrs.Commands;

public class SetActiveNoteCommand : IRequest
{
    public int Id { get; set; }
}
