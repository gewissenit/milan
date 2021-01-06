#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation.Events
{
  public abstract class ProductsRelatedEndEvent : ProductsRelatedEvent, IRelatedEvent
  {
    public ProductsRelatedEndEvent(IEntity sender, string eventName, IEnumerable<Product> relatedProducts, ISimulationEvent relatedStartEvent)
      : base(sender, eventName, relatedProducts)
    {
      RelatedEvent = relatedStartEvent;
    }

    public virtual double Duration
    {
      get { return ScheduledTime - RelatedEvent.ScheduledTime; }
    }
  }
}