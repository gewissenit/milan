#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore.Tests;
using Milan.Simulation.Logging;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using NUnit.Framework;
using Rhino.Mocks;


namespace Milan.Simulation.Tests
{
  [TestFixture]
  public abstract class SchedulerObserverFixtureBase<T> : DomainEntityFixture<T>
    where T : SchedulerObserver
  {
    [SetUp]
    public virtual void SetUp()
    {
      _mockRepo = new MockRepository();
      _mockModel = _mockRepo.DynamicMock<IModel>();
      _mockExperiment = _mockRepo.DynamicMock<IExperiment>();
      _mockScheduler = _mockRepo.DynamicMock<IScheduler>();
    }

    [TearDown]
    public virtual void TearDown()
    {
      _mockRepo = null;
      _mockModel = null;
      _mockExperiment = null;
      _mockScheduler = null;
    }

    protected abstract void Default_Ctor(T sut);
    protected MockRepository _mockRepo;
    protected IModel _mockModel;
    protected IExperiment _mockExperiment;
    protected IScheduler _mockScheduler;

    [Test]
    public virtual void Set_IsEnabled()
    {
      const bool value = true;

      var sut = CreateMinimalConfiguredSUT();
      var loggerMock = _mockRepo.DynamicMock<IExperimentLogWriterProvider>();
      _mockExperiment.Expect(x => x.LogProvider)
                      .Return(loggerMock);

      _mockExperiment.Expect(e => e.Scheduler)
                      .Return(_mockScheduler)
                      .Repeat.Any();
      _mockRepo.ReplayAll();
      sut.Configure(_mockExperiment);
      sut.Model = _mockModel;


      SetProperty(sut, "IsEnabled", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.IsEnabled = v, s => s.IsEnabled);
      _mockRepo.VerifyAll();
    }


    [Test]
    public void Set_Model()

    {
      var value = _mockRepo.DynamicMock<IModel>();
      SetProperty("Model", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Model = v, s => s.Model);
    }

    [Test]
    public virtual void Set_Same_IsEnabled_Does_Not_Raise_PropertyChanged()
    {
      const bool value = true;
      var sut = CreateMinimalConfiguredSUT();
      _mockExperiment.Expect(e => e.Scheduler)
                      .Return(_mockScheduler)
                      .Repeat.Any();
      var loggerMock = _mockRepo.DynamicMock<IExperimentLogWriterProvider>();
      _mockExperiment.Expect(x => x.LogProvider)
                      .Return(loggerMock);
      _mockRepo.ReplayAll();
      sut.Configure(_mockExperiment);
      sut.Model = _mockModel;

      SetPropertyTwice(sut, value, (s, v) => s.IsEnabled = v);
    }


    [Test]
    public void Set_Same_Model_Does_Not_Raise_PropertyChanged()
    {
      var value = _mockRepo.DynamicMock<IModel>();
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Model = v);
    }
  }
}