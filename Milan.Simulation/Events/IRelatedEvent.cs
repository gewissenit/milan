#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion



namespace Milan.Simulation.Events
{
  public interface IRelatedEvent : ISimulationEvent
  {
    ISimulationEvent RelatedEvent { get; }

    double Duration { get; }
  }
}