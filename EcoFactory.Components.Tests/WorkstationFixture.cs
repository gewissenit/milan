#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EcoFactory.Components.Events;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Resources;
using Milan.Simulation.Resources.Events;
using Milan.Simulation.Tests;
using Moq;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class WorkstationFixture : WorkstationBaseFixture<Workstation>
  {
    protected override Workstation CreateSUT()
    {
      return new Workstation();
    }

    [Test]
    public void Cancel_Setup_On_Failure_And_Then_Setups_Again_After_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(20);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(3);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.HasSetup = true;
      sut.CanFail = true;

      sut.Initialize();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      sut.IsInState("Setup");
      Assert.AreEqual(4, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1));
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<SetupEndEvent>();
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<SetupCancelEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Cancel_Setup_On_ShutDown_And_Then_Setups_Again_After_ReStartUp()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(5);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;
      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.HasSetup = true;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      sut.IsInState("Setup");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1));
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Setup");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<SetupEndEvent>();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.IsEventTypeScheduled<SetupCancelEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Setup");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Destroy_Processing_Products_On_Failure()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.IsNotAvailableFor(product1);

      sut.Initialize();

      sut.IsAvailableFor(product1);
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductsDestroyedEvent>(product1));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Do_Not_Destroy_Blocked_Products_On_Failure()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product = MockProduct(modelMock, productType);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(5);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(10);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(product))
                   .Return(false)
                   .Repeat.Once();
      successorMock.Expect(se => se.IsAvailable(product))
                   .Return(true)
                   .Repeat.Twice();

      successorMock.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      _entities.Add(successorMock);
      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.Add(connectionMock);

      sut.IsNotAvailableFor(product);

      sut.Initialize();

      sut.IsAvailableFor(product);
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      sut.Receive(product);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<BlockedEndEvent>();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(15, scheduler.Clock.CurrentTime);
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      eventRaiser.Raise(sut.Connections.First(), EventArgs.Empty);
      sut.IsInState("Idle");
      Assert.AreEqual(4, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product));
      scheduler.IsEventTypeScheduled<BlockedEndEvent>();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Do_Not_Setup_But_Process_Products_If_ProductType_Stays_Same()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productA2 = MockProduct(modelMock, productTypeA);
      var productA3 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(5);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(5);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(em => em.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(cm => cm.Destination)
                    .Return(successorMock);
      connectionMock.Expect(cm => cm.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(sm => sm.IsAvailable(productA1))
                   .Return(true)
                   .Repeat.Twice();

      _entities.Add(successorMock);
      _entities.Add(productTypeA);
      _entities.Add(productTypeB);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.HasSetup = true;
      sut.Add(connectionMock);
      sut.IsNotAvailableFor(productA1);

      sut.Initialize();
      sut.IsAvailableFor(productA1);

      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsInState("Setup");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsInState("Setup");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<SetupEndEvent>();

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(productA1));

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      sut.IsAvailableFor(productB1);
      sut.IsAvailableFor(productA2);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.Receive(productA2);
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA3);
      sut.IsInState("Processing");
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA2));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(productA2));
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Uses_ProductType_Specific_Resources()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var batchSizeDistributionMock = new Mock<IRealDistribution>();
      var productType = new Mock<IProductType>();
      var product1 = new Mock<Product>(modelMock.Object, productType.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);

      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      batchSizeDistributionMock.Setup(rdm => rdm.GetSample())
                               .Returns(1);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      product1.Setup(p => p.ProductType)
              .Returns(productType.Object);

      resourceTypeAmount.Setup(rta => rta.Amount)
                        .Returns(5);
      resourceTypeAmount.Setup(rta => rta.ResourceType)
                        .Returns(resourceTypeMock.Object);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });

      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 5))
                      .Returns(true);

      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 5))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object,
                                 resource.Object,
                                 resource.Object,
                                 resource.Object,
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock.Object;
      sut.BatchSizeDistribution = batchSizeDistributionMock.Object;
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });
      sut.ProductTypeSpecificProcessingResourcesDictionary.Add(productType.Object, new Dictionary<IResourcePool, IDictionary<IResourceType, int>>
                                                                                   {
                                                                                     {
                                                                                       resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                                                                {
                                                                                                                  {
                                                                                                                    resourceTypeMock.Object, 5
                                                                                                                  }
                                                                                                                }
                                                                                     }
                                                                                   });

      sut.IsNotAvailableFor(product1.Object);

      sut.Initialize();

      sut.IsAvailableFor(product1.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1.Object);

      sut.IsInState("Processing");
      Assert.AreEqual(5, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1.Object));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1.Object));

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Enter_Processing_When_Resources_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var batchSizeDistributionMock = new Mock<IRealDistribution>();
      var productType = new Mock<IProductType>();
      var product1 = new Mock<Product>(modelMock.Object, productType.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);

      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      batchSizeDistributionMock.Setup(rdm => rdm.GetSample())
                               .Returns(1);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      product1.Setup(p => p.ProductType)
              .Returns(productType.Object);

      resourceTypeAmount.Setup(rta => rta.Amount)
                        .Returns(5);
      resourceTypeAmount.Setup(rta => rta.ResourceType)
                        .Returns(resourceTypeMock.Object);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });

      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(true);

      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 1))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock.Object;
      sut.BatchSizeDistribution = batchSizeDistributionMock.Object;
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });

      sut.IsNotAvailableFor(product1.Object);

      sut.Initialize();

      sut.IsAvailableFor(product1.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1.Object);

      sut.IsInState("Processing");
      Assert.AreEqual(5, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1.Object));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1.Object));

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Enter_Processing_When_Resources_Got_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var batchSizeDistributionMock = new Mock<IRealDistribution>();
      var productType = new Mock<IProductType>();
      var product1 = new Mock<Product>(modelMock.Object, productType.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var resourceTypeAmount = new Mock<IResourceTypeAmount>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);

      product1.Setup(p => p.ProductType)
              .Returns(productType.Object);

      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);

      batchSizeDistributionMock.Setup(rdm => rdm.GetSample())
                               .Returns(1);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      resourcePoolMock.Setup(rp => rp.Resources)
                      .Returns(new List<IResourceTypeAmount>()
                               {
                                 resourceTypeAmount.Object
                               });
      resourcePoolMock.Setup(rp => rp.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(false);
      resourcePoolMock.Setup(rp => rp.GetResources(resourceTypeMock.Object, 1))
                      .Returns(new List<Resource>()
                               {
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock.Object;
      sut.BatchSizeDistribution = batchSizeDistributionMock.Object;
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });

      _mockRepo.ReplayAll();

      sut.IsNotAvailableFor(product1.Object);

      sut.Initialize();

      sut.IsAvailableFor(product1.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1.Object);

      sut.IsInState("Awaiting Processing Resources");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      resourcePoolMock.Setup(r => r.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(true);

      resourcePoolMock.Raise(rp => rp.ResourcesReceived += null, EventArgs.Empty);

//      eventRaiser.Raise(resourcePoolMock, EventArgs.Empty);

      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());

      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1.Object));

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Get_Failed_While_Off_And_Switches_To_Idle_After_ReStartUp_And_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(10);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(10);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.CanFail = true;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Get_Failed_While_Off_And_Switches_To_Off_After_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(10);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;
      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.CanFail = true;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Get_Resources_After_Setup_And_Enter_Processing()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var batchSizeDistributionMock = new Mock<IRealDistribution>();
      var setupDurationDistributionMock = new Mock<IRealDistribution>();
      var productType = new Mock<IProductType>();
      var modelMock = new Mock<IModel>();
      var product1 = new Mock<Product>(modelMock.Object, productType.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();
      var del = new Func<double>(() => 0);
      var resource = new Mock<Resource>(resourceTypeMock.Object, resourcePoolMock.Object, del);
      product1.Setup(p => p.ProductType)
              .Returns(productType.Object);

      processingDurationDistributionMock.Setup(rdm => rdm.GetSample())
                                        .Returns(5);
      batchSizeDistributionMock.Setup(rdm => rdm.GetSample())
                               .Returns(1);
      setupDurationDistributionMock.Setup(sddm => sddm.GetSample())
                                   .Returns(5);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      resourcePoolMock.Setup(r => r.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(true);

      resourcePoolMock.Setup(x => x.GetResources(resourceTypeMock.Object, 1))
                      .Returns(new[]
                               {
                                 resource.Object
                               });

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock.Object;
      sut.BatchSizeDistribution = batchSizeDistributionMock.Object;
      sut.SetupDurationDistribution = setupDurationDistributionMock.Object;
      sut.HasSetup = true;
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });

//      _MockRepo.ReplayAll();

      sut.IsNotAvailableFor(product1.Object);

      sut.Initialize();

      sut.IsAvailableFor(product1.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1.Object);

      sut.IsInState("Setup");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1.Object));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<SetupStartEvent>(product1.Object));


      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<SetupEndEvent>(product1.Object));

      scheduler.ProcessNextSchedulable();

      sut.IsInState("Processing");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceReceivedEvent>();
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();


      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1.Object));

      sut.IsInState("Processing");
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      Assert.AreEqual(0, scheduler.Count());

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Available_After_Shift_Is_Started()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsAvailableFor(product1);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Blocked_After_ReStartUp_When_Was_Blocked_Before_ShutDown()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;
      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.IsEventTypeScheduled<BlockedEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Idle_After_ReStartUp_When_Was_Idle_Before_ShutDown()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;
      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Is_Not_Available_When_Shift_Is_Not_Started()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;
      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsNotAvailableFor(product1);
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Process_Products_One_After_Another()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();

      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);
      var product2 = MockProduct(modelMock, productType);
      var product3 = MockProduct(modelMock, productType);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(5);
      batchSizeDistributionMock.Expect(rdm => rdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(product1))
                   .Return(false)
                   .Repeat.Once();
      successorMock.Expect(se => se.IsAvailable(product1))
                   .Return(true)
                   .Repeat.Twice();

      _entities.Add(successorMock);
      _entities.Add(productType);
      successorMock.GotAvailable += null;
      LastCall.IgnoreArguments();
      var eventRaiser = LastCall.GetEventRaiser();

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.Add(connectionMock);
      sut.IsNotAvailableFor(product1);

      sut.Initialize();
      sut.IsAvailableFor(product1);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1);
      sut.IsNotAvailableFor(product2);
      sut.IsInState("Processing");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product1));

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(product2);
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(product2);
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(product2);
      Assert.AreEqual(0, scheduler.Count());

      eventRaiser.Raise(sut.Connections.First(), EventArgs.Empty);
      sut.IsAvailableFor(product2);
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product1));
      scheduler.IsEventTypeScheduled<BlockedEndEvent>();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsAvailableFor(product2);
      sut.IsInState("Idle");
      Assert.AreEqual(0, scheduler.Count());
      sut.Receive(product2);
      sut.IsNotAvailableFor(product3);
      sut.IsInState("Processing");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(product2));
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product2));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Setup_And_Then_Process_Products_If_ProductType_Changed()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var setupDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeA = _mockRepo.DynamicMock<IProductType>();
      var productTypeB = _mockRepo.DynamicMock<IProductType>();
      var productA1 = MockProduct(modelMock, productTypeA);
      var productA2 = MockProduct(modelMock, productTypeA);
      var productA3 = MockProduct(modelMock, productTypeA);
      var productB1 = MockProduct(modelMock, productTypeB);
      var productB2 = MockProduct(modelMock, productTypeB);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(5);
      setupDurationDistributionMock.Expect(sddm => sddm.GetSample())
                                   .Return(5);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(em => em.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(cm => cm.Destination)
                    .Return(successorMock);
      connectionMock.Expect(cm => cm.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(sm => sm.IsAvailable(productA1))
                   .Return(true)
                   .Repeat.Twice();

      _entities.Add(successorMock);
      _entities.Add(productTypeA);
      _entities.Add(productTypeB);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.SetupDurationDistribution = setupDurationDistributionMock;
      sut.HasSetup = true;
      sut.Add(connectionMock);
      sut.IsNotAvailableFor(productA1);

      sut.Initialize();
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      sut.IsInState("Idle");
      sut.IsAvailableFor(productA1);

      scheduler.ProcessNextSchedulable();
      sut.Receive(productA1);
      sut.IsNotAvailableFor(productA2);
      sut.IsNotAvailableFor(productB1);
      sut.IsInState("Setup");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productA1));
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsInState("Setup");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<SetupEndEvent>();

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingStartEvent>(productA1));

      scheduler.ProcessNextSchedulable();
      sut.IsNotAvailableFor(productB1);
      sut.IsNotAvailableFor(productA3);
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      sut.IsAvailableFor(productB1);
      sut.IsAvailableFor(productA2);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.Receive(productB1);
      sut.IsNotAvailableFor(productA2);
      sut.IsNotAvailableFor(productB2);
      sut.IsInState("Setup");
      Assert.AreEqual(10, scheduler.Clock.CurrentTime);
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(productB1));
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.IsEventTypeScheduled<SetupStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_Failed_On_ShutDown_And_Switches_To_Idle_After_ReStartUp_And_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(20);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_Failed_On_ShutDown_And_Switches_To_Off_After_Failure_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);
      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(20);

      _entities.Add(productType);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleEndEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      sut.OnWorkingTimeEnded();
      sut.IsInState("Failure");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Awaiting_Resources_When_Resources_Not_Available()
    {
      var scheduler = new SpyScheduler();

      var modelMock = new Mock<IModel>();
      var experimentMock = new Mock<IExperiment>();
      var processingDurationDistributionMock = new Mock<IRealDistribution>();
      var batchSizeDistributionMock = new Mock<IRealDistribution>();
      var productType = new Mock<IProductType>();
      var product1 = new Mock<Product>(modelMock.Object, productType.Object, double.NaN);
      var resourcePoolMock = new Mock<IResourcePool>();
      var resourceTypeMock = new Mock<IResourceType>();

      product1.Setup(p => p.ProductType)
              .Returns(productType.Object);

      batchSizeDistributionMock.Setup(rdm => rdm.GetSample())
                               .Returns(1);

      experimentMock.Setup(x => x.Scheduler)
                    .Returns(scheduler);

      resourcePoolMock.Setup(r => r.HasAvailable(resourceTypeMock.Object, 1))
                      .Returns(false);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock.Object;
      sut.Model = modelMock.Object;
      sut.BatchSizeDistribution = batchSizeDistributionMock.Object;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock.Object;
      sut.ProcessingResourcesDictionary.Add(resourcePoolMock.Object, new Dictionary<IResourceType, int>
                                                                     {
                                                                       {
                                                                         resourceTypeMock.Object, 1
                                                                       }
                                                                     });
      _mockRepo.ReplayAll();

      sut.IsNotAvailableFor(product1.Object);

      sut.Initialize();

      sut.IsAvailableFor(product1.Object);
      sut.IsInState("Idle");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1.Object);

      sut.IsInState("Awaiting Processing Resources");
      Assert.AreEqual(3, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductReceiveEvent>(product1.Object));
      scheduler.IsEventTypeScheduled<ResourceRequestedEvent>();

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_On_ShutDown_And_ReStartup_And_Switches_To_Blocked_After_Processing_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(11);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Blocked");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_On_ShutDown_And_ReStartup_And_Switches_To_Idle_After_Processing_End()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(11);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(cm => cm.Destination)
                    .Return(successorMock);
      connectionMock.Expect(cm => cm.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(sm => sm.IsAvailable(product1))
                   .Return(true)
                   .Repeat.Twice();

      _mockRepo.ReplayAll();

      _entities.Add(productType);
      _entities.Add(successorMock);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.Add(connectionMock);
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product1));
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_On_ShutDown_And_Switches_To_Off_After_Processing_End_And_Sending_Products_And_Switches_To_Idle_After_ReStartUp()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      connectionMock.Expect(cm => cm.Destination)
                    .Return(successorMock);
      connectionMock.Expect(cm => cm.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(sm => sm.IsAvailable(product1))
                   .Return(true)
                   .Repeat.Twice();

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(6);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);
      _entities.Add(successorMock);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.Add(connectionMock);
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product1));
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_On_ShutDown_And_Switches_To_Off_After_Processing_End_And_Switches_To_Blocked_After_ReStartUp()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);

      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(6);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Blocked");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<BlockedStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void Stay_In_Processing_On_ShutDown_And_Switches_To_Off_After_Processing_End_And_Switches_To_Idle_After_ReStartUp_And_Sending_Products()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var product1 = MockProduct(modelMock, productType);
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      connectionMock.Expect(cm => cm.Destination)
                    .Return(successorMock);
      connectionMock.Expect(cm => cm.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(sm => sm.IsAvailable(product1))
                   .Return(false)
                   .Repeat.Once();
      successorMock.Expect(sm => sm.IsAvailable(product1))
                   .Return(true)
                   .Repeat.Twice();
      processingDurationDistributionMock.Expect(pddm => pddm.GetSample())
                                        .Return(6);
      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _mockRepo.ReplayAll();

      _entities.Add(productType);
      _entities.Add(successorMock);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.IsWorkingTimeDependent = true;

      sut.Model = modelMock;
      sut.Add(connectionMock);
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;

      sut.Initialize();
      sut.IsInState("Off");
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      sut.Receive(product1);
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeEnded();
      sut.IsInState("Processing");
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Off");
      Assert.AreEqual(1, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffStartEvent>();
      scheduler.ProcessNextSchedulable();
      scheduler.Clock.AdvanceTime(4);
      sut.OnWorkingTimeStarted();
      sut.IsInState("Idle");
      Assert.AreEqual(3, scheduler.Count());
      scheduler.IsEventTypeScheduled<OffEndEvent>();
      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductTransmitEvent>(product1));
      scheduler.ProcessNextSchedulable();
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.ProcessNextSchedulable();
      _mockRepo.VerifyAll();
    }

    [Test]
    public void While_Processing_Failure_Return_Resources()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var processingDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureDurationDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var failureOccurenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var resourcePoolMock = _mockRepo.DynamicMock<IResourcePool>();
      var resourceTypeMock = _mockRepo.DynamicMock<IResourceType>();

      var influenceMock = _mockRepo.DynamicMock<IInfluence>();
      var rtiMock = _mockRepo.DynamicMock<IResourceTypeInfluence>();
      resourceTypeMock.Expect(rtm => rtm.Influences)
                      .Return(new[]
                              {
                                rtiMock
                              });
      rtiMock.Expect(rtim => rtim.Influence)
             .Return(influenceMock);
      _mockRepo.ReplayAll();
      var del = new Func<double>(() => 0);
      var resource = _mockRepo.DynamicMock<Resource>(resourceTypeMock, resourcePoolMock, del);

      var product1 = MockProduct(modelMock, productType);

      processingDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                        .Return(20);

      failureDurationDistributionMock.Expect(rdm => rdm.GetSample())
                                     .Return(5);

      failureOccurenceDistributionMock.Expect(rdm => rdm.GetSample())
                                      .Return(5);

      batchSizeDistributionMock.Expect(bsdm => bsdm.GetSample())
                               .Return(1);

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      resourcePoolMock.Expect(r => r.HasAvailable(resourceTypeMock, 1))
                      .Return(true);
      resourcePoolMock.Expect(x => x.GetResources(resourceTypeMock, 1))
                      .Return(new[]
                              {
                                resource
                              });

      _entities.Add(resourcePoolMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.ProcessingDurationDistribution = processingDurationDistributionMock;
      sut.FailureDurationDistribution = failureDurationDistributionMock;
      sut.FailureOccurrenceDistribution = failureOccurenceDistributionMock;
      sut.CanFail = true;
      sut.IsNotAvailableFor(product1);

      sut.ProcessingResourcesDictionary.Add(resourcePoolMock, new Dictionary<IResourceType, int>
                                                              {
                                                                {
                                                                  resourceTypeMock, 1
                                                                }
                                                              });

      sut.Initialize();
      sut.IsAvailableFor(product1);
      sut.IsInState("Idle");
      Assert.AreEqual(2, scheduler.Count());
      scheduler.IsEventTypeScheduled<IdleStartEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();

      sut.Receive(product1);

      Assert.IsTrue(sut.AvailableProcessingResources.Count() == 1);
      Assert.IsTrue(sut.AvailableProcessingResources.Any(apr => apr.ResourceType == resourceTypeMock));

      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      scheduler.ProcessNextSchedulable();
      sut.IsInState("Processing");
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProcessingEndEvent>(product1));
      scheduler.IsEventTypeScheduled<FailureStartEvent>();

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);

      sut.IsInState("Failure");

      Assert.IsTrue(!sut.AvailableProcessingResources.Any());

      Assert.AreEqual(4, scheduler.Count());
      scheduler.IsEventTypeScheduled<FailureEndEvent>();
      scheduler.IsEventTypeScheduled<FailureStartEvent>();
      scheduler.IsEventTypeScheduled<ResourceReleasedEvent>();
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ProductsDestroyedEvent>(product1));

      _mockRepo.VerifyAll();
    }
  }
}