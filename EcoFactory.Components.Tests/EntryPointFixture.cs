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
using Milan.Simulation.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  public class EntryPointFixture : StationaryElementFixture<EntryPoint>
  {
    protected override EntryPoint CreateSUT()
    {
      return new EntryPoint();
    }

    protected override void Default_Ctor(EntryPoint sut)
    {
      Assert.AreEqual(null, sut.BatchSize);
      Assert.IsEmpty(sut.ArrivalOccurrences.ToArray());
      Assert.IsEmpty(sut.BatchSizes.ToArray());
    }

    [Test]
    public void Add_ArrivalOccurrence()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddArrival(item);
      Assert.Contains(item, sut.ArrivalOccurrences.ToArray());
      Assert.AreEqual(1, sut.ArrivalOccurrences.Count());
    }

    [Test]
    public void Add_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      Assert.Contains(item, sut.BatchSizes.ToArray());
      Assert.AreEqual(1, sut.BatchSizes.Count());
    }

    [Test]
    public void Create_Missed_If_Successor_Is_Not_Available()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var arrivalOccurrenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      arrivalOccurrenceDistributionMock.Expect(rdm => rdm.GetSample())
                                       .Return(5);
      batchSizeDistributionMock.Expect(rdm => rdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(null))
                   .IgnoreArguments()
                   .Return(false)
                   .Repeat.Once();

      _entities.Add(successorMock);
      _entities.Add(productTypeMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.ArrivalsDictionary.Add(productTypeMock, arrivalOccurrenceDistributionMock);
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.Add(connectionMock);

      sut.Initialize();

      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductMissedEvent>(productTypeMock));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Create_ThroughputStarted_If_Successor_Is_Available()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var arrivalOccurrenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      var successorMock = _mockRepo.DynamicMock<IStationaryElement>();
      var connectionMock = _mockRepo.DynamicMock<IConnection>();

      arrivalOccurrenceDistributionMock.Expect(rdm => rdm.GetSample())
                                       .Return(5);
      batchSizeDistributionMock.Expect(rdm => rdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      connectionMock.Expect(c => c.Destination)
                    .Return(successorMock);
      connectionMock.Expect(c => c.ProductTypes)
                    .Return(new List<IProductType>());
      successorMock.Expect(se => se.IsAvailable(null))
                   .IgnoreArguments()
                   .Return(true)
                   .Repeat.Twice();

      _entities.Add(successorMock);
      _entities.Add(productTypeMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.ArrivalsDictionary.Add(productTypeMock, arrivalOccurrenceDistributionMock);
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.Add(connectionMock);

      sut.Initialize();

      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));

      scheduler.ProcessNextSchedulable();
      Assert.AreEqual(5, scheduler.Clock.CurrentTime);
      Assert.AreEqual(2, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ThroughputStartEvent>(productTypeMock));
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Fail_On_Add_ArrivalOccurrence_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddArrival(item);
      Assert.Throws<InvalidOperationException>(() => sut.AddArrival(item));
    }

    [Test]
    public void Fail_On_Add_BatchSize_Twice()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      Assert.Throws<InvalidOperationException>(() => sut.AddBatchSize(item));
    }

    [Test]
    public void Fail_On_Add_Second_ArrivalOccurrence_With_Same_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var item2 = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      sut.AddArrival(item);
      sut.AddArrival(item2);
      Assert.Throws<InvalidOperationException>(()=>_mockRepo.VerifyAll());
    }

    [Test]
    public void Fail_On_Add_Second_BatchSize_With_Same_ProductType()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var item2 = _mockRepo.DynamicMock<IProductTypeDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();
      item.Expect(i => i.ProductType)
          .Return(productTypeMock);
      item2.Expect(i => i.ProductType)
           .Return(productTypeMock);
      sut.AddBatchSize(item);
      sut.AddBatchSize(item2);
      Assert.Throws<InvalidOperationException>(() => _mockRepo.VerifyAll());
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_ArrivalOccurrence()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveArrival(item));
    }

    [Test]
    public void Fail_On_Remove_Non_Existing_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveBatchSize(item));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_ArrivalOccurrence()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveArrival(null));
    }

    [Test]
    public void Fail_On_Remove_Null_Fro_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      Assert.Throws<InvalidOperationException>(() => sut.RemoveBatchSize(null));
    }

    [Test]
    public void Remove_ArrivalOccurrence()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddArrival(item);
      sut.RemoveArrival(item);
      Assert.IsEmpty(sut.ArrivalOccurrences.ToArray());
    }

    [Test]
    public void Remove_BatchSize()
    {
      var sut = CreateMinimalConfiguredSUT();
      var item = _mockRepo.DynamicMock<IProductTypeDistribution>();
      sut.AddBatchSize(item);
      sut.RemoveBatchSize(item);
      Assert.IsEmpty(sut.BatchSizes.ToArray());
    }

    [Test]
    public void Schedule_First_Arrivals_After_First_Shift_Is_Started()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var arrivalOccurrenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();

      arrivalOccurrenceDistributionMock.Expect(rdm => rdm.GetSample())
                                       .Return(5);
      batchSizeDistributionMock.Expect(rdm => rdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      _entities.Add(productTypeMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.ArrivalsDictionary.Add(productTypeMock, arrivalOccurrenceDistributionMock);
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.IsWorkingTimeDependent = true;

      sut.Initialize();

      Assert.AreEqual(0, scheduler.Count());
      sut.OnWorkingTimeStarted();
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Set_BatchSize()
    {
      var value = _mockRepo.DynamicMock<DistributionConfiguration>();
      SetProperty("BatchSize", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.BatchSize = v, s => s.BatchSize);
    }


    [Test]
    public void Set_Same_BatchSize_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.DynamicMock<DistributionConfiguration>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.BatchSize = v);
    }


    [Test]
    public void Suspend_Arrivals_When_Shift_Is_Ended_And_Resume_After_ReStartup()
    {
      var scheduler = new SpyScheduler();

      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var arrivalOccurrenceDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var batchSizeDistributionMock = _mockRepo.DynamicMock<IRealDistribution>();
      var productTypeMock = _mockRepo.DynamicMock<IProductType>();

      arrivalOccurrenceDistributionMock.Expect(rdm => rdm.GetSample())
                                       .Return(10);
      batchSizeDistributionMock.Expect(rdm => rdm.GetSample())
                               .Return(1);
      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);
      experimentMock.Expect(x => x.CurrentTime)
                    .Return(2);

      _entities.Add(productTypeMock);

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;
      sut.Model = modelMock;
      sut.BatchSizeDistribution = batchSizeDistributionMock;
      sut.IsWorkingTimeDependent = true;
      sut.ArrivalsDictionary.Add(productTypeMock, arrivalOccurrenceDistributionMock);

      sut.Initialize();

      Assert.AreEqual(0, scheduler.Count());
      sut.OnWorkingTimeStarted();
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));
      scheduler.Clock.AdvanceTime(2);
      sut.OnWorkingTimeEnded();
      Assert.AreEqual(0, scheduler.Count());
      sut.OnWorkingTimeStarted();
      Assert.AreEqual(2, scheduler.Clock.CurrentTime);
      Assert.AreEqual(1, scheduler.Count());
      Assert.AreEqual(1, scheduler.GetCountForContainingProductType<ProductsArrivedEvent>(productTypeMock));
      Assert.AreEqual(1, scheduler.GetCountForScheduledTime<ProductsArrivedEvent>(10));

      _mockRepo.VerifyAll();
    }
  }
}