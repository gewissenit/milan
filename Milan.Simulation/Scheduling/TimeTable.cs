#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public abstract class TimeTable : ITimeTable
  {
    private readonly List<ISimulationEvent> _schedulablesList = new List<ISimulationEvent>();

    protected List<ISimulationEvent> SchedulablesList
    {
      get { return _schedulablesList; }
    }

    public virtual ISimulationEvent First
    {
      get { return SchedulablesList.First(); }
    }

    public virtual ISimulationEvent Last
    {
      get { return SchedulablesList.Last(); }
    }

    public int Insert(ISimulationEvent schedulable)
    {
      if (IsEmpty())
      {
        SchedulablesList.Add(schedulable);
        return SchedulablesList.Count - 1;
      }
      return InsertOnStrategy(schedulable);
    }

    public void InsertAfter(ISimulationEvent predecessor, ISimulationEvent schedulable)
    {
      SchedulablesList.Insert(SchedulablesList.IndexOf(predecessor) + 1, schedulable);
    }

    public virtual void InsertAsFirst(ISimulationEvent schedulable)
    {
      SchedulablesList.Insert(0, schedulable);
    }

    public virtual void InsertAsLast(ISimulationEvent schedulable)
    {
      SchedulablesList.Add(schedulable);
    }

    public void InsertBefore(ISimulationEvent successor, ISimulationEvent schedulable)
    {
      SchedulablesList.Insert(SchedulablesList.IndexOf(successor), schedulable);
    }

    public bool IsEmpty()
    {
      return !SchedulablesList.Any();
    }

    public virtual ISimulationEvent GetSuccessorOf(ISimulationEvent origin)
    {
      if (origin == SchedulablesList[SchedulablesList.Count - 1])
      {
        throw new InvalidOperationException("ISchedulable has no successor!");
      }
      return SchedulablesList[SchedulablesList.IndexOf(origin) + 1];
    }

    public virtual ISimulationEvent GetPredecessorOf(ISimulationEvent origin)
    {
      if (origin == SchedulablesList[0])
      {
        throw new InvalidOperationException("ISchedulable has no predecessor!");
      }
      return SchedulablesList[SchedulablesList.IndexOf(origin) - 1];
    }

    public virtual void Remove(ISimulationEvent schedulable)
    {
      SchedulablesList.Remove(schedulable);
    }

    public virtual void RemoveFirst()
    {
      SchedulablesList.RemoveAt(0);
    }

    public bool Contains(ISimulationEvent schedulable)
    {
      return SchedulablesList.Contains(schedulable);
    }

    public void Clear()
    {
      SchedulablesList.Clear();
    }

    public IEnumerable<ISimulationEvent> ScheduledItems
    {
      get { return SchedulablesList.AsEnumerable(); }
    }

    public abstract int InsertOnStrategy(ISimulationEvent schedulable);

    public void RemoveAt(int index)
    {
      SchedulablesList.RemoveAt(index);
    }
  }
}