using System;

namespace Emporer.WPF
{
  public interface ISelection
  {
    /// <summary>
    /// Selects the given object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selection"></param>
    /// <param name="subscriber"></param>
    void Select<T>(T selection, object subscriber);

    /// <summary>
    /// Subscribes to selection change that change the selection to an instance of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscriber"></param>
    /// <param name="onSelection"></param>
    void Subscribe<T>(object subscriber, Action<T> onSelection);

    /// <summary>
    /// Unsubscribes all subscriptions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscriber"></param>
    void Unsubscribe<T>(object subscriber);

    /// <summary>
    /// The currently selected object.
    /// </summary>
    object Current { get; }
  }
}