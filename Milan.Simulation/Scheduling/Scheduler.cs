#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public class Scheduler : IScheduler
  {
    public Scheduler(IClock clock, ITimeTable timeTable)
    {
      Clock = clock;
      TimeTable = timeTable;
    }

    private ITimeTable TimeTable { get; set; }
    public IClock Clock { get; private set; }

    public IEnumerable<ISimulationEvent> ScheduledItems
    {
      get { return TimeTable.ScheduledItems; }
    }

    public bool IsScheduled(ISimulationEvent schedulable)
    {
      return TimeTable.Contains(schedulable);
    }

    public void RemoveSchedulable(ISimulationEvent simEvent)
    {
      TimeTable.Remove(simEvent);
      RaiseContentChanged(simEvent, NotifyCollectionChangedAction.Remove);
    }

    /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
    public void Schedule(ISimulationEvent schedulable, double timeDifference)
    {
      if (timeDifference < 0)
      {
        throw new InvalidOperationException(
          string.Format("The given time difference ({0}) results in an event in the past. Time difference parameter has to be positive.",
                        timeDifference));
      }
      schedulable.ScheduledTime = Clock.CurrentTime + timeDifference;
      Schedule(schedulable);
    }

    public void ScheduleAfter(ISimulationEvent simEvent, ISimulationEvent predecessor)
    {
      simEvent.ScheduledTime = predecessor.ScheduledTime;
      TimeTable.InsertAfter(predecessor, simEvent);
      RaiseContentChanged(simEvent, NotifyCollectionChangedAction.Add);
    }

    public void ScheduleAfterCurrent(ISimulationEvent simEvent)
    {
      if (CurrentSchedulable == null)
      {
        simEvent.ScheduledTime = 0;
        TimeTable.InsertAsFirst(simEvent);
      }
      else
      {
        simEvent.ScheduledTime = CurrentSchedulable.ScheduledTime;
        TimeTable.InsertAfter(CurrentSchedulable, simEvent);
      }
      RaiseContentChanged(simEvent, NotifyCollectionChangedAction.Add);
    }

    public void ScheduleBefore(ISimulationEvent simEvent, ISimulationEvent successor)
    {
      simEvent.ScheduledTime = successor.ScheduledTime;
      TimeTable.InsertBefore(successor, simEvent);
      RaiseContentChanged(simEvent, NotifyCollectionChangedAction.Add);
    }

    /// <exception cref="InvalidOperationException">Could not reschedule. Schedulable is not yet scheduled!</exception>
    public void ReSchedule(ISimulationEvent simEvent, double timeDifference)
    {
      if (!IsScheduled(simEvent))
      {
        throw new InvalidOperationException("Could not reschedule. Schedulable is not yet scheduled!");
      }
      if (CurrentSchedulable == simEvent)
      {
        throw new InvalidOperationException("Could not reschedule. Schedulable is CurrentSchedulable!");
      }
      RemoveSchedulable(simEvent);
      Schedule(simEvent, timeDifference);
    }

    /// <exception cref="InvalidOperationException">Could not reschedule. Schedulable is not yet scheduled!</exception>
    public void ReScheduleBefore(ISimulationEvent simEvent, ISimulationEvent successor)
    {
      if (!IsScheduled(simEvent))
      {
        throw new InvalidOperationException("Could not reschedule. Schedulable is not yet scheduled!");
      }
      if (CurrentSchedulable == simEvent)
      {
        throw new InvalidOperationException("Could not reschedule. Schedulable is CurrentSchedulable!");
      }
      if (CurrentSchedulable == successor)
      {
        throw new InvalidOperationException("Could not reschedule before CurrentSchedulable!");
      }
      RemoveSchedulable(simEvent);
      ScheduleBefore(simEvent, successor);
    }

    /// <exception cref="InvalidOperationException">Could not reschedule. Scheduled item is not yet scheduled!</exception>
    public void ReScheduleAfter(ISimulationEvent simEvent, ISimulationEvent predecessor)
    {
      if (!IsScheduled(simEvent))
      {
        throw new InvalidOperationException("Could not reschedule. Scheduled item is not yet scheduled!");
      }
      if (CurrentSchedulable == simEvent)
      {
        throw new InvalidOperationException("Could not reschedule. Scheduled item is the current scheduled item!");
      }
      RemoveSchedulable(simEvent);
      ScheduleAfter(simEvent, predecessor);
    }

    public virtual void ProcessNextSchedulable()
    {
      if (!CanProcessNextSchedulable())
      {
        return;
      }
      CurrentSchedulable = TimeTable.First;
      Clock.AdvanceTime(CurrentSchedulable.ScheduledTime - Clock.CurrentTime);
      RaiseBeforeSchedulableHandling(CurrentSchedulable);
      CurrentSchedulable.Handle();
      RaiseSchedulableHandled();
      if (!TimeTable.IsEmpty())
      {
        TimeTable.Remove(CurrentSchedulable);
      }
    }

    public bool CanProcessNextSchedulable()
    {
      return !TimeTable.IsEmpty();
    }

    public ISimulationEvent CurrentSchedulable { get; private set; }
    public event EventHandler<SchedulerEventArgs> SchedulableHandled;
    public event EventHandler<SchedulerEventArgs> BeforeSchedulableHandling;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public IEnumerator<ISimulationEvent> GetEnumerator()
    {
      return TimeTable.ScheduledItems.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <exception cref="ArgumentNullException"><paramref name="simEvent" /> is <c>null</c>.</exception>
    private void Schedule(ISimulationEvent simEvent)
    {
      if (simEvent == null)
      {
        throw new ArgumentNullException("simEvent");
      }
      TimeTable.Insert(simEvent);
      simEvent.InsertedTime = Clock.CurrentTime;
      RaiseContentChanged(simEvent, NotifyCollectionChangedAction.Add);
    }

    private void RaiseBeforeSchedulableHandling(ISimulationEvent simulationEvent)
    {
      if (BeforeSchedulableHandling != null)
      {
        BeforeSchedulableHandling(this, new SchedulerEventArgs(simulationEvent));
      }
    }

    private void RaiseSchedulableHandled()
    {
      if (SchedulableHandled != null)
      {
        SchedulableHandled(this, new SchedulerEventArgs(CurrentSchedulable));
      }
    }

    protected void RaiseContentChanged(object item, NotifyCollectionChangedAction notifyCollectionChangedAction)
    {
      if (CollectionChanged != null)
      {
        CollectionChanged(this, new NotifyCollectionChangedEventArgs(notifyCollectionChangedAction, item));
      }
    }
  }
}