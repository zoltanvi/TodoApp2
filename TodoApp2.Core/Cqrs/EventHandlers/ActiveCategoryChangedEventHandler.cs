using MediatR;
using Modules.Categories.Contracts.Cqrs.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApp2.Core.Cqrs.EventHandlers;

public class ActiveCategoryChangedEventHandler : INotificationHandler<ActiveCategoryChangedEvent>
{
    public static event Action<ActiveCategoryChangedEvent> ActiveCategoryChanged;

    public Task Handle(ActiveCategoryChangedEvent notification, CancellationToken cancellationToken)
    {
        ActiveCategoryChanged?.Invoke(notification);

        return Task.CompletedTask;
    }
}
