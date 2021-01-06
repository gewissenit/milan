#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public class LinearTimeTable : TimeTable
  {
    /// linear strategy to find the place for the new EventNote
    public override int InsertOnStrategy(ISimulationEvent schedulable)
    {
      // current position in list
      var index = 0;
      var listCount = SchedulablesList.Count;

      while ((index < listCount) &&
             (SchedulablesList[index].ScheduledTime <= schedulable.ScheduledTime))
      {
        index++;
      }
      SchedulablesList.Insert(index, schedulable);
      return index;
    }
  }
}