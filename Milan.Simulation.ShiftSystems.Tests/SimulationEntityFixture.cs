#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.ShiftSystems.Tests
{
  public abstract class SimulationEntityFixture
  {
    protected IExperiment _discreteExperimentMock;
    protected MockRepository _mockRepository;
    protected IModel _modelMock;
    protected SpyScheduler _schedulerSpy;

    [SetUp]
    public virtual void Setup()
    {
      _mockRepository = new MockRepository();
    }

    [TearDown]
    public virtual void TearDown()
    {
      _mockRepository = null;
    }

    protected ISimulationEvent RaiseExpectedEvent(string eventName)
    {
      Assert.AreEqual(_schedulerSpy.ScheduledItems.First()
                                    .Name,
                      eventName);
      _schedulerSpy.ProcessNextSchedulable();
      var schedulable = _schedulerSpy.ProcessedItems.Last();
      Assert.AreEqual(eventName, schedulable.Name);
      return schedulable;
    }


    protected ISimulationEvent RaiseExpectedEvent(string eventName, double expectedTime)
    {
      var schedulable = RaiseExpectedEvent(eventName);
      Assert.AreEqual(expectedTime,
                      schedulable.ScheduledTime,
                      string.Format("Event '{0}' was scheduled {1:0.00} min off the expected time ({2:0.00})",
                                    eventName,
                                    (expectedTime - schedulable.ScheduledTime) / 60000,
                                    expectedTime));
      return schedulable;
    }


    /// <summary>
    ///   Creates the experiment environment with strict scheduler.
    /// </summary>
    protected void CreateExperimentEnvironmentWithSpyScheduler(IEntity sut, IEnumerable<IEntity> entitiesInModel)
    {
      _discreteExperimentMock = _mockRepository.DynamicMock<IExperiment>();
      _schedulerSpy = new SpyScheduler();
      _modelMock = _mockRepository.DynamicMock<IModel>();
      _discreteExperimentMock.Expect(mockExperiment => mockExperiment.Scheduler)
                              .Return(_schedulerSpy);
      _discreteExperimentMock.Expect(mockExperiment => mockExperiment.Model)
                              .Return(_modelMock)
                              .Repeat.Any();
      sut.CurrentExperiment = _discreteExperimentMock;
      sut.Model = _modelMock;

      _modelMock.Expect(m => m.Entities)
                 .Return(entitiesInModel.Concat(new[]
                                                {
                                                  sut
                                                }));
    }

    protected void CreateExperimentEnvironmentWithSpyScheduler(IEntity sut)
    {
      CreateExperimentEnvironmentWithSpyScheduler(sut, new IEntity[0]);
    }
  }
}