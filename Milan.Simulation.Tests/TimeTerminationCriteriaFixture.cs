#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Milan.Simulation.Scheduling;
using NUnit.Framework;
using Rhino.Mocks;

namespace Milan.Simulation.Tests
{
  [TestFixture]
  public class TimeTerminationCriteriaFixture : SchedulerObserverFixtureBase<TimeTerminationCriteria>
  {
    protected override TimeTerminationCriteria CreateSUT()
    {
      return new TimeTerminationCriteria();
    }

    protected override void Default_Ctor(TimeTerminationCriteria sut)
    {
    }

    public override void Set_IsEnabled()
    {
      // this does not work for this observer
    }

    [Test]
    public void Schedule_Simulation_End_At_Stop_Time()
    {
      var startTime = DateTime.Now;
      var stopTime = startTime.AddDays(28);
      var durationReal = stopTime.Subtract(startTime);
      var durationSim = durationReal.ToSimulationTimeSpan();

      var schedulerMock = _mockRepo.DynamicMock<IScheduler>();
      var experimentMock = _mockRepo.DynamicMock<IExperiment>();
      var modelMock = _mockRepo.DynamicMock<IModel>();

      experimentMock.Expect(x => x.Scheduler)
                    .Return(schedulerMock);

      schedulerMock.Expect(sm => sm.Schedule(Arg<SimulationEndEvent>.Is.NotNull, Arg<double>.Is.Equal(durationSim)));

      _mockRepo.ReplayAll();

      var sut = CreateMinimalConfiguredSUT();
      sut.Configure(experimentMock);
      sut.Model = modelMock;
      sut.Duration = durationReal;

      experimentMock.Raise(m => m.Started += null, null, EventArgs.Empty);

      _mockRepo.VerifyAll();
    }

    [Test]
    public void Set_Duration()
    {
      var value = TimeSpan.FromDays(1);
      SetProperty("Duration", value, (s, v) => Assert.IsTrue(s == v), (s, v) => s.Duration = v, s => s.Duration);
    }


    [Test]
    public void Set_Same_Duration_Does_Not_Raise_PropertyChanged()
    {
      var value = TimeSpan.FromDays(1);
      SetPropertyTwice(CreateMinimalConfiguredSUT(), value, (s, v) => s.Duration = v);
    }
  }
}