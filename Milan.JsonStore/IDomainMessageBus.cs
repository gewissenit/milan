using System;

namespace Milan.JsonStore
{
  public interface IDomainMessageBus
  {
    IObservable<PropertyChange> PropertyChanges { get; }
    IObservable<object> EntityAdded { get; }
    IObservable<object> EntityRemoved { get; }
  }
}