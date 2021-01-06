#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Events;
using Milan.Simulation.Scheduling;

namespace Milan.Simulation.Tests
{
  public class SpyScheduler : Scheduler
  {
    private readonly Queue<ISimulationEvent> _processedItems = new Queue<ISimulationEvent>();

    public SpyScheduler()
      : base(new Clock(), new BinaryTimeTable())
    {
    }

    public IEnumerable<ISimulationEvent> ProcessedItems
    {
      get { return _processedItems; }
    }

    public override void ProcessNextSchedulable()
    {
      base.ProcessNextSchedulable();
      _processedItems.Enqueue(CurrentSchedulable);
    }
  }
}