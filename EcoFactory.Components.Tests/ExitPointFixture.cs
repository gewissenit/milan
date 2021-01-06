#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace EcoFactory.Components.Tests
{
  [TestFixture]
  internal class ExitPointFixture : StationaryElementFixture<ExitPoint>
  {
    protected override ExitPoint CreateSUT()
    {
      return new ExitPoint();
    }

    [Test]
    public void Is_Allways_Available()
    {
      var sut = CreateMinimalConfiguredSUT();
      var productType = _mockRepo.DynamicMock<IProductType>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var productMock = MockProduct(modelMock, productType);
      Assert.IsTrue(sut.IsAvailable(productMock));
    }

    [Test]
    public void Receive_One_Product_And_Schedule_TransformationEnd()
    {
      var scheduler = new SpyScheduler();

      var productType = _mockRepo.DynamicMock<IProductType>();
      var modelMock = _mockRepo.DynamicMock<IModel>();
      var productMock = MockProduct(modelMock, productType);
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(scheduler);

      var sut = CreateMinimalConfiguredSUT();
      sut.CurrentExperiment = experimentMock;

      _mockRepo.ReplayAll();

      sut.Initialize();

      sut.Receive(productMock);
      Assert.AreEqual(1, scheduler.GetCountForContainingProduct<ThroughputEndEvent>(productMock));

      _mockRepo.VerifyAll();
    }
  }
}