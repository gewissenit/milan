#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public class ExperimentObserver : SchedulerObserver
  {
    private readonly IList<Position> _statisticPositions = new List<Position>();

    public TimeSpan ExperimentRunTime { get; private set; }
    public DateTime ExperimentStartDate { get; private set; }
    public TimeSpan SimulatedDuration { get; private set; }
    public DateTime SimulatedStartDate { get; private set; }

    public IEnumerable<Position> StatisticPositions
    {
      get { return _statisticPositions; }
    }

    protected override void AdditionalInitialization()
    {
      SimulatedStartDate = Model.StartDate;
      ExperimentStartDate = DateTime.Now;
    }
    
    protected override void OnEventOccurred(ISimulationEvent e)
    {
      CreatePosition(e);
    }

    private void CreatePosition(ISimulationEvent e)
    {
      var end = e.ScheduledTime.ToRealDate(SimulatedStartDate);
      var station = e.Sender is IEntity
                      ? (IEntity) e.Sender
                      : null;
      _statisticPositions.Add(new Position
                              {
                                Station = station,
                                Event = e.Id,
                                Process = e.GetType()
                                           .Name,
                                Experiment = CurrentExperiment,
                                EndDate = end
                              });
    }

    public override void Reset()
    {
      _statisticPositions.Clear();
      base.Reset();
    }

    protected override void OnSimulationEnd(ISimulationEvent endEvent)
    {
      ExperimentRunTime = DateTime.Now - ExperimentStartDate;
      SimulatedDuration = CurrentExperiment.CurrentTime.ToRealTimeSpan();
    }
  }
}