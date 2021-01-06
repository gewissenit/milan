#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Resources
{
  public interface IReceiver<T> : IEntity
  {
    void Receive(T message);
    bool IsAvailable(T message);
  }
}