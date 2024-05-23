using System;

namespace TodoApp2.Core;

/// <summary>
/// Marker interface for view models.
/// See: <see cref="IPropertyChangeNotifier"/>.
/// </summary>
public interface IBaseViewModel : IPropertyChangeNotifier, IDisposable
{
}