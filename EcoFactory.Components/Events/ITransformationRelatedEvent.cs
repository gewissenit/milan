#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Events;

namespace EcoFactory.Components.Events
{
  public interface ITransformationRelatedEvent : ISimulationEvent
  {
    ITransformationRule TransformationRule { get; }
  }
}