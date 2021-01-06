#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components;
using Emporer.Material;
using Emporer.Unit;
using Milan.Simulation.Events;
using Milan.Simulation.Logging;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using Moq;
using NUnit.Framework;

namespace Milan.Simulation.MaterialAccounting.Tests
{
  [TestFixture]
  public class ProductExitedMaterialStatisticsFixture
  {
    private Mock<IProductType> MockProductType(string productTypeName)
    {
      var productType = new Mock<IProductType>();

      productType.Setup(p => p.Name)
                 .Returns(productTypeName);

      return productType;
    }

    private Mock<Product> MockProduct(IProductType productType, Mock<IModel> modelMock)
    {
      modelMock.Setup(m => m.GetIndexForDynamicEntity(typeof(Product)))
               .Returns(20);
      var product = new Mock<Product>(modelMock.Object, productType, 0);
      product.Setup(p => p.ProductType)
             .Returns(productType);

      return product;
    }

    private Mock<IWorkstation> MockWorkstation(string name)
    {
      var workstation = new Mock<IWorkstation>();

      workstation.Setup(ws => ws.Name)
                 .Returns(name);

      return workstation;
    }

    private Mock<ThroughputEndEvent> MockTroughputEndEvent(IEntity sender, IEnumerable<Product> products)
    {
      var throughputEndEvent = new Mock<ThroughputEndEvent>(sender, products);

      throughputEndEvent.Setup(e => e.Products)
                        .Returns(products);

      return throughputEndEvent;
    }

    private Mock<ThroughputStartEvent> MockTroughputStartEvent(IEntity sender, IEnumerable<Product> products)
    {
      var mockEvent = new Mock<ThroughputStartEvent>(sender, products);

      mockEvent.Setup(e => e.Products)
               .Returns(products);

      return mockEvent;
    }

    private Mock<IMaterial> MockMaterial()
    {
      var material = new Mock<IMaterial>();
      return material;
    }

    private Mock<IUnit> MockUnit(string symbol)
    {
      var mockU = new Mock<IUnit>();
      mockU.SetupProperty(u => u.Symbol, symbol);
      return mockU;
    }

    [Test]
    public void BalancePosition_Is_Used_During_ThroughputEndEvent()
    {
      var experiment = new Mock<IExperiment>();
      var logProvider = new Mock<IExperimentLogWriterProvider>();
      var scheduler = new Mock<IScheduler>();

      const string productTypeName = "pt1";
      const string ws1 = "ws1";
      var modelMock = new Mock<IModel>();
      modelMock.Setup(m => m.StartDate)
               .Returns(DateTime.Now);
      var productTypeMock = MockProductType(productTypeName);
      var productMock = MockProduct(productTypeMock.Object, modelMock);
      var workstationMock = MockWorkstation(ws1);
      var throughputStartEventMock = MockTroughputStartEvent(workstationMock.Object, new[]
                                                                                     {
                                                                                       productMock.Object
                                                                                     });
      var throughputEndEventMock = MockTroughputEndEvent(workstationMock.Object, new[]
                                                                                 {
                                                                                   productMock.Object
                                                                                 });
      experiment.Setup(e => e.Scheduler)
                .Returns(scheduler.Object);
      experiment.Setup(e => e.LogProvider)
                .Returns(logProvider.Object);

      throughputStartEventMock.Setup(e => e.Sender)
                              .Returns(workstationMock.Object);
      throughputStartEventMock.Setup(e => e.Products)
                              .Returns(new[]
                                       {
                                         productMock.Object
                                       });

      throughputEndEventMock.Setup(e => e.Sender)
                            .Returns(workstationMock.Object);
      throughputEndEventMock.Setup(e => e.Products)
                            .Returns(new[]
                                     {
                                       productMock.Object
                                     });

      var sut = new ThroughputObserver();
      sut.Model = modelMock.Object;
      sut.Configure(experiment.Object);

      sut.IsEnabled = true;
      sut.Initialize();

      scheduler.Raise(s => s.SchedulableHandled += null, new SchedulerEventArgs(throughputStartEventMock.Object));
      scheduler.Raise(s => s.SchedulableHandled += null, new SchedulerEventArgs(throughputEndEventMock.Object));

      Assert.IsTrue(sut.StatisticPositions.Any());
    }
  }
}