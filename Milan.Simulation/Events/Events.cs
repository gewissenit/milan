#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation.Events
{
  public class ProductTransmitEvent : ProductsRelatedEvent
  {
    private const string EventName = "Product Transmit";


    public ProductTransmitEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ProductReceiveEvent : ProductsRelatedEvent
  {
    private const string EventName = "Product Receive";


    public ProductReceiveEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ThroughputStartEvent : ProductsRelatedEvent
  {
    private const string EventName = "Throughput Start";


    public ThroughputStartEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ThroughputEndEvent : ProductsRelatedEvent
  {
    private const string EventName = "Throughput End";


    public ThroughputEndEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }

  public class ProductsDestroyedEvent : ThroughputEndEvent
  {
    private const string EventName = "Products Destroyed";

    public ProductsDestroyedEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, relatedProducts)
    {
      Name = EventName;
    }
  }

  public class ProductMissedEvent : ProductsRelatedEvent
  {
    private const string EventName = "Product Missed";

    public ProductMissedEvent(IEntity sender, IEnumerable<Product> relatedProducts)
      : base(sender, EventName, relatedProducts)
    {
    }
  }
}