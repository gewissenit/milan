#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public interface ITimeTable
  {
    ISimulationEvent First { get; }
    ISimulationEvent Last { get; }
    IEnumerable<ISimulationEvent> ScheduledItems { get; }

    int Insert(ISimulationEvent schedulable);

    void InsertAfter(ISimulationEvent predecessor, ISimulationEvent schedulable);

    void InsertAsFirst(ISimulationEvent schedulable);

    void InsertAsLast(ISimulationEvent schedulable);

    void InsertBefore(ISimulationEvent successor, ISimulationEvent schedulable);

    bool IsEmpty();

    ISimulationEvent GetPredecessorOf(ISimulationEvent origin);

    ISimulationEvent GetSuccessorOf(ISimulationEvent origin);

    void Remove(ISimulationEvent schedulable);

    void RemoveFirst();

    bool Contains(ISimulationEvent target);

    void Clear();
  }
}