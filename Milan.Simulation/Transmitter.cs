#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation
{
  public class Transmitter<T>
    where T : class
  {
    /// <exception cref="ArgumentNullException"><paramref name="receivers" /> is <c>null</c>.</exception>
    public Transmitter(IEnumerable<IReceiver<T>> receivers)
    {
      if (receivers == null)
      {
        throw new ArgumentNullException("receivers");
      }

      Receivers = receivers.ToArray();
      OnTransmit = () =>
                   {
                   };
    }


    internal IEnumerable<IReceiver<T>> Receivers { get; set; }
    public Action OnTransmit { get; set; }

    /// <exception cref="ArgumentNullException"><paramref name="message" /> is <c>null</c>.</exception>
    public virtual bool CanTransmit(T message)
    {
      if (message == null)
      {
        throw new ArgumentNullException("message");
      }
      return Receivers.Any(receiver => receiver.IsAvailable(message));
    }


    protected virtual IReceiver<T> SelectReveiver(T message)
    {
      return Receivers.First(r => r.IsAvailable(message));
    }


    /// <exception cref="ArgumentNullException"><paramref name="message" /> is <c>null</c>.</exception>
    public void Transmit(T message)
    {
      if (message == null)
      {
        throw new ArgumentNullException("message");
      }
      SelectReveiver(message)
        .Receive(message);
      OnTransmit();
    }
  }
}