#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.Events
{
  public abstract class TransformationRelatedEndEvent : TransformationRelatedEvent, IRelatedEvent
  {
    public TransformationRelatedEndEvent(IEntity sender,
                                         string eventName,
                                         IEnumerable<Product> relatedProducts,
                                         ISimulationEvent relatedStartEvent,
                                         ITransformationRule relatedTransformationRule)
      : base(sender, eventName, relatedProducts, relatedTransformationRule)
    {
      RelatedEvent = relatedStartEvent;
    }

    public double Duration
    {
      get { return ScheduledTime - RelatedEvent.ScheduledTime; }
    }
  }
}