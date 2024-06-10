using MediatR;

namespace Modules.Tasks.Contracts.Cqrs.Queries;

public class GetTaskReorderIndexQuery : IRequest<int>
{
    public object TaskObject { get; set; }
}
