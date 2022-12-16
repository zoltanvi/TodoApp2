using System;

namespace TodoApp2.Core
{
    public interface IAsyncActionService
    {
        void InvokeAsync(Action action);
    }
}
