#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public interface IScheduler : INotifyCollectionChanged, IEnumerable<ISimulationEvent>
  {
    IEnumerable<ISimulationEvent> ScheduledItems { get; }
    ISimulationEvent CurrentSchedulable { get; }
    IClock Clock { get; }

    void Schedule(ISimulationEvent schedulable, double timeDifference);

    void ScheduleAfter(ISimulationEvent schedulable, ISimulationEvent predecessor);

    void ScheduleAfterCurrent(ISimulationEvent schedulable);

    void ScheduleBefore(ISimulationEvent schedulable, ISimulationEvent successor);

    void ReSchedule(ISimulationEvent schedulable, double timeDifference);

    void ReScheduleBefore(ISimulationEvent schedulable, ISimulationEvent successor);

    void ReScheduleAfter(ISimulationEvent schedulable, ISimulationEvent predecessor);

    void RemoveSchedulable(ISimulationEvent schedulable);

    bool IsScheduled(ISimulationEvent schedulable);

    bool CanProcessNextSchedulable();

    void ProcessNextSchedulable();

    event EventHandler<SchedulerEventArgs> BeforeSchedulableHandling;

    event EventHandler<SchedulerEventArgs> SchedulableHandled;
  }
}