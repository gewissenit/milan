#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation;
using Milan.Simulation.Events;

namespace EcoFactory.Components.Events
{
  public abstract class TransformationRelatedEvent : ProductsRelatedEvent, ITransformationRelatedEvent
  {
    public TransformationRelatedEvent(IEntity sender,
                                      string eventName,
                                      IEnumerable<Product> relatedProducts,
                                      ITransformationRule relatedTransformationRule)
      : base(sender, eventName, relatedProducts)
    {
      TransformationRule = relatedTransformationRule;
    }

    public ITransformationRule TransformationRule { get; private set; }
  }
}