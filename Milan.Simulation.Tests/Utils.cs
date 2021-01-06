#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Moq;

namespace Milan.Simulation.Tests
{
  public static class Utils
  {
    public static Mock<ThroughputEndEvent> At(this Mock<ThroughputEndEvent> endEvent, Mock<IStationaryElement> entity)
    {
      endEvent.Setup(e => e.Sender)
              .Returns(entity.Object);
      return endEvent;
    }

    public static Mock<ThroughputEndEvent> WithProducts(this Mock<ThroughputEndEvent> endEvent, IEnumerable<Mock<Product>> productMocks)
    {
      endEvent.Setup(e => e.Products)
              .Returns(productMocks.Select(p => p.Object));
      return endEvent;
    }

    public static Mock<ThroughputStartEvent> At(this Mock<ThroughputStartEvent> endEvent, Mock<IStationaryElement> entity)
    {
      endEvent.Setup(e => e.Sender)
              .Returns(entity.Object);
      return endEvent;
    }

    public static Mock<ThroughputStartEvent> WithProducts(this Mock<ThroughputStartEvent> endEvent, IEnumerable<Mock<Product>> productMocks)
    {
      endEvent.Setup(e => e.Products)
              .Returns(productMocks.Select(p => p.Object));
      return endEvent;
    }

    public static int GetCountForScheduledTime<TProductsRelatedEventType>(this SpyScheduler scheduler, double time)
      where TProductsRelatedEventType : ISimulationEvent
    {
      return scheduler.OfType<TProductsRelatedEventType>()
                      .Count(e => e.ScheduledTime == time);
    }

    public static int GetCountForContainingProductType<TProductsRelatedEventType>(this SpyScheduler scheduler, IProductType productType)
      where TProductsRelatedEventType : IProductsRelatedEvent
    {
      return scheduler.OfType<TProductsRelatedEventType>()
                      .SelectMany(pde => pde.Products)
                      .Count(p => p.ProductType == productType);
    }

    public static int GetCountForContainingProduct<TProductsRelatedEventType>(this SpyScheduler scheduler, Product product)
      where TProductsRelatedEventType : IProductsRelatedEvent
    {
      return scheduler.OfType<TProductsRelatedEventType>()
                      .Select(pde => pde.Products)
                      .Count(p => p.Contains(product));
    }
  }
}