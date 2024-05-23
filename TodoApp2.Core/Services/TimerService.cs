using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace TodoApp2.Core;

public class TimerService
{
    private const string NotFoundErrorMessage = "Timer not found.";
    private const int DefaultInterval = 1000;
    private readonly Dictionary<Guid, DispatcherTimer> _timers;
    private readonly Dictionary<Guid, EventHandler> _eventHandlers;

    public static TimerService Instance { get; } = new TimerService();

    private TimerService()
    {
        _timers = new Dictionary<Guid, DispatcherTimer>();
        _eventHandlers = new Dictionary<Guid, EventHandler>();
    }

    public Guid CreateTimer(int intervalMilliseconds, EventHandler tickEventHandler, bool start = false)
    {
        var guid = Guid.NewGuid();

        DispatcherTimer timer = new DispatcherTimer();
        timer.Tick += tickEventHandler;
        timer.Interval = TimeSpan.FromMilliseconds(intervalMilliseconds);

        if (start)
        {
            timer.Start();
        }

        _timers.Add(guid, timer);
        _eventHandlers.Add(guid, tickEventHandler);

        return guid;
    }

    public Guid CreateTimer(EventHandler tickEventHandler, bool start = false)
    {
        return CreateTimer(DefaultInterval, tickEventHandler, start);
    }

    public void DeleteTimer(Guid guid)
    {
        if (_timers.TryGetValue(guid, out var timer) &&
            _eventHandlers.TryGetValue(guid, out var eventHandler))
        {
            timer.Tick -= eventHandler;
            timer.Stop();

            _timers.Remove(guid);
            _eventHandlers.Remove(guid);
        }
        else
        {
            throw new ArgumentException(NotFoundErrorMessage);
        }
    }

    public void ModifyTimerInterval(Guid guid, int intervalMilliseconds)
    {
        if (_timers.TryGetValue(guid, out var timer))
        {
            timer.Interval = TimeSpan.FromMilliseconds(intervalMilliseconds);
        }
        else
        {
            throw new ArgumentException(NotFoundErrorMessage);
        }
    }

    public void StartTimer(Guid guid)
    {
        if (_timers.TryGetValue(guid, out var timer))
        {
            timer.Start();
        }
        else
        {
            throw new ArgumentException(NotFoundErrorMessage);
        }
    }

    public void RestartTimer(Guid guid)
    {
        if (_timers.TryGetValue(guid, out var timer))
        {
            timer.Stop();
            timer.Start();
        }
        else
        {
            throw new ArgumentException(NotFoundErrorMessage);
        }
    }

    public void StopTimer(Guid guid)
    {
        if (_timers.TryGetValue(guid, out var timer))
        {
            timer.Stop();
        }
        else
        {
            throw new ArgumentException(NotFoundErrorMessage);
        }
    }
}
