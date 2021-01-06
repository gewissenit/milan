#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Linq;
using Milan.Simulation.Observers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class ProductTerminationCriteriaFixture : SchedulerObserverFixtureBase<ProductTerminationCriteria>
  {
    protected override ProductTerminationCriteria CreateSUT()
    {
      return new ProductTerminationCriteria();
    }

    protected override void Default_Ctor(ProductTerminationCriteria sut)
    {
      Assert.IsEmpty(sut.ProductAmounts.ToArray());
    }

    public override void Set_IsEnabled()
    {
      const bool value = true;

      var sut = CreateMinimalConfiguredSUT();
      _mockExperiment.Expect(e => e.Scheduler)
                      .Return(_mockScheduler)
                      .Repeat.Any();

      _mockModel.Expect(m => m.Entities)
                 .Return(new IEntity[0])
                 .Repeat.Any();
      _mockRepo.ReplayAll();
      sut.Configure(_mockExperiment);
      sut.Model = _mockModel;


      SetProperty(sut, "IsEnabled", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.IsEnabled = v, s => s.IsEnabled);
      _mockRepo.VerifyAll();
    }

    public override void Set_Same_IsEnabled_Does_Not_Raise_PropertyChanged()
    {
      const bool value = true;
      var sut = CreateMinimalConfiguredSUT();
      _mockExperiment.Expect(e => e.Scheduler)
                      .Return(_mockScheduler)
                      .Repeat.Any();
      _mockModel.Expect(m => m.Entities)
                 .Return(new IEntity[0])
                 .Repeat.Any();
      _mockRepo.ReplayAll();
      sut.Configure(_mockExperiment);
      sut.Model = _mockModel;

      SetPropertyTwice(sut, value, (s, v) => s.IsEnabled = v);
    }
  }
}