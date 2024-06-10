using MediatR;

namespace Modules.Categories.Contracts.Cqrs.Events;

public class ActiveCategoryChangedEvent : INotification
{
    public string OldName { get; set; }
    public string NewName { get; set; }
}
