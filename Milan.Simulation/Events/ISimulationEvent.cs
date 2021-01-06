#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Events
{
  public interface ISimulationEvent
  {
    string Name { get; }

    double ScheduledTime { get; set; }

    double InsertedTime { get; set; }

    object Sender { get; }

    Action<ISimulationEvent> OnOccur { get; set; }

    long Id { get; }
    void Handle();
  }
}