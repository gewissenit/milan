#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation
{
  public interface IReceiver<T>
  {
    void Receive(T message);
    event EventHandler<EventArgs> GotAvailable;
    bool IsAvailable(T message);
  }
}