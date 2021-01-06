#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Logging;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using Moq;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Milan.Simulation.Tests
{
  //todo: change test names and add some more tests (see coverage)
  [TestFixture]
  public class ThroughputObserverFixture
  {
    private int _id;
    private Mock<IExperiment> _mockExperiment;
    private Mock<IScheduler> _mockScheduler;
    private Mock<IModel> _mockModel;

    [SetUp]
    public void Setup()
    {
      _mockExperiment = new Mock<IExperiment>();
      _mockScheduler = new Mock<IScheduler>();
      _mockModel = new Mock<IModel>();
      var logger = new Mock<IExperimentLogWriterProvider>();

      _mockExperiment.Setup(ex => ex.LogProvider)
                     .Returns(logger.Object);
      _mockExperiment.Setup(ex => ex.Scheduler)
                     .Returns(_mockScheduler.Object);
      _mockModel.Setup(ex => ex.StartDate)
                     .Returns(DateTime.Now);
    }

    [TearDown]
    public void TearDown()
    {
      _mockScheduler = null;
      _mockExperiment = null;
    }


    private Mock<ThroughputEndEvent> MockThroughputEndEvent()
    {
      var mockEvent = new Mock<ThroughputEndEvent>(null, new Product[]
                                                         {
                                                           null
                                                         });
      return mockEvent;
    }

    private Mock<ThroughputStartEvent> MockThroughputStartEvent()
    {
      var mockEvent = new Mock<ThroughputStartEvent>(null, new Product[]
                                                         {
                                                           null
                                                         });
      return mockEvent;
    }

    private int GetId()
    {
      return _id++;
    }

    private Mock<Product> MockProduct(Mock<IModel> model, Mock<IProductType> mockPT)
    {
      model.Setup(m => m.GetIndexForDynamicEntity(typeof(Product)))
           .Returns(GetId());
      var mockP = new Mock<Product>(model.Object, mockPT.Object, double.NaN);
      mockP.Setup(p => p.ProductType)
           .Returns(mockPT.Object);
      return mockP;
    }

    private Mock<Product> MockProduct(string productType)
    {
      var mockPT = MockProductType(productType);
      var mockModel = new Mock<IModel>();
      var mockP = MockProduct(mockModel, mockPT);
      return mockP;
    }

    private Mock<IProductType> MockProductType(string productType)
    {
      var mockPT = new Mock<IProductType>().SetupProperty(pt => pt.Name, productType);
      return mockPT;
    }
    
    private Mock<IStationaryElement> MockStation(string exitPoint)
    {
      var mockXP = new Mock<IStationaryElement>();
      mockXP.SetupProperty(se => se.Name, exitPoint);
      return mockXP;
    }

    private void RaiseEvent(ISimulationEvent simulationEvent)
    {
      _mockScheduler.Raise(s => s.SchedulableHandled += null, new SchedulerEventArgs(simulationEvent));
    }
    
    [Test]
    public void Configure___Sets_CurrentExperiment_Makes_Scheduler_Available()
    {
      CreateSUT();
    }

    private ThroughputObserver CreateSUT()
    {
      var SUT = new ThroughputObserver();
      SUT.Model = _mockModel.Object;
      SUT.Configure(_mockExperiment.Object);
      SUT.IsEnabled = true;
      return SUT;
    }

    [Test]
    public void If_Triggered_Once_Creates_A_New_MaterialBalance()
    {
      var SUT = CreateSUT();

      const string ex1 = "EX1";
      const string pt1 = "PT1";

      var mockXP = MockStation(ex1);
      var mockP = MockProduct(pt1);
      var startEvent = MockThroughputStartEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP
                      });
      var endEvent = MockThroughputEndEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP
                      });
      
      SUT.Initialize();
      RaiseEvent(startEvent.Object);
      RaiseEvent(endEvent.Object);

      Assert.IsNotEmpty(SUT.StatisticPositions.ToArray());
      Assert.AreEqual(1, SUT.StatisticPositions.Count());
    }

    [Test]
    public void If_Triggered_Once_With_Composed_Products_Accumulates_Integrated_Materials()
    {
      var SUT = CreateSUT();

      // 1 product containing 2 equal integrated products
      const string ex1 = "EX1";
      const string pt1 = "PT1";

      var mockXP = MockStation(ex1);

      var mockP1 = MockProduct(pt1);
      
      var startEvent = MockThroughputStartEvent()
         .At(mockXP)
         .WithProducts(new[]
                       {
                        mockP1
                       });
      var endEvent = MockThroughputEndEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP1
                      });
      
      SUT.Initialize();
      RaiseEvent(startEvent.Object);
      RaiseEvent(endEvent.Object);

      Assert.AreEqual(1, SUT.StatisticPositions.Count());
    }

    [Test]
    public void If_Triggered_Once_With_Different_Materials_Accumulates_Both_Materials()
    {
      var SUT = CreateSUT();

      const string ex1 = "EX1";
      const string pt1 = "PT1";

      var mockXP = MockStation(ex1);
      
      var mockP = MockProduct(pt1);
      var startEvent = MockThroughputStartEvent()
         .At(mockXP)
         .WithProducts(new[]
                       {
                        mockP
                       });
      var endEvent = MockThroughputEndEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP
                      });
      
      SUT.Initialize();
      RaiseEvent(startEvent.Object);
      RaiseEvent(endEvent.Object);

      Assert.AreEqual(1, SUT.StatisticPositions.Count());
    }

    [Test]
    public void If_Triggered_Twice_With_Same_Materials_Accumulates_Into_One_Balance()
    {
      var SUT = CreateSUT();

      const string ex1 = "EX1";
      const string pt1 = "PT1";

      var mockXP = MockStation(ex1);
      
      var mockP = MockProduct(pt1);
      var mockP2 = MockProduct("pt2");
      var startEvent1 = MockThroughputStartEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP
                      });
      var endEvent1 = MockThroughputEndEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP
                      });

      var startEvent2 = MockThroughputStartEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP2
                      });
      var endEvent2 = MockThroughputEndEvent()
        .At(mockXP)
        .WithProducts(new[]
                      {
                        mockP2
                      });
      
      SUT.Initialize();
      RaiseEvent(startEvent1.Object);
      RaiseEvent(endEvent1.Object);
      RaiseEvent(startEvent2.Object);
      RaiseEvent(endEvent2.Object);
      
      Assert.AreEqual(2, SUT.StatisticPositions.Count());
    }

    [Test]
    public void Initialize___Starts_With_Empty_Successful()
    {
      var SUT = CreateSUT();
      
      Assert.IsNotNull(SUT.StatisticPositions);
      Assert.IsEmpty(SUT.StatisticPositions.ToArray());
    }

    [Test]
    public void Is_Not_Triggered_If_Event_Is_No_ThroughputEnd()
    {
      var SUT = CreateSUT();

      var mockXP = MockStation("An exit point name");
      var mockEvent = new Mock<SimulationEvent>(mockXP.Object, "Some event type name");
      mockEvent.Setup(e => e.Sender)
               .Returns(mockXP.Object);

      RaiseEvent(mockEvent.Object);
      Assert.IsEmpty(SUT.StatisticPositions.ToArray());
    }

    [Test]
    public void Is_Not_Triggered_If_Sender_Is_No_ExitPoint()
    {
      var SUT = CreateSUT();

      var mockEvent = MockThroughputEndEvent();
      mockEvent.Setup(e => e.Sender)
               .Returns(new Mock<IEntity>().Object);

      RaiseEvent(mockEvent.Object);
      Assert.IsEmpty(SUT.StatisticPositions.ToArray());
    }
  }
}

// ReSharper restore InconsistentNaming