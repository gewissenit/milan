#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public class InterpolatedTimeTable : TimeTable
  {
    public override int InsertOnStrategy(ISimulationEvent schedulable)
    {
      if (IsEmpty())
      {
        SchedulablesList.Add(schedulable);
        return SchedulablesList.Count - 1;
      }

      var refTime = schedulable.ScheduledTime;

      var leftTime = First.ScheduledTime;
      if (leftTime > refTime)
      {
        SchedulablesList.Insert(0, schedulable);
        return 0;
      }

      var rightTime = Last.ScheduledTime;
      if (rightTime <= refTime)
      {
        SchedulablesList.Add(schedulable);
        return SchedulablesList.Count - 1;
      }

      // left border of search partition
      var left = 0;
      // right border of search partition
      var right = SchedulablesList.Count - 1;
      do
      {
        var index = (int) (left + (refTime - leftTime) / (rightTime - leftTime) * (right - left));
        // check if EventNote at index has smaller or equal time
        if (SchedulablesList[index].ScheduledTime <= refTime)
        {
          if (index < (SchedulablesList.Count - 1))
          {
            // is there a note to the right
            left = index + 1;
            leftTime = SchedulablesList[left].ScheduledTime;
            if (leftTime > refTime)
            {
              // if note to the right is larger
              SchedulablesList.Insert(left, schedulable);
              // found position
              return left;
            }

            // no hit, so set new boundaries and go on
          } // no EventNotes right of the index, so all
          else
          {
            // notes are smaller, thus append to end
            SchedulablesList.Add(schedulable);
            return SchedulablesList.Count - 1;
          }
        }
        else
        {
          // EventNote at index has larger time
          if (index > 0)
          {
            // is there a note left of the index?
            right = index - 1;
            rightTime = SchedulablesList[right].ScheduledTime;
            if (rightTime <= refTime)
            {
              // if note to the left is smallerOrEqual
              SchedulablesList.Insert(index, schedulable);
              // found position
              return index;
            }

            // no hit, so set new boundaries and go on
          } // no EventNotes left of the index, so all
          else
          {
            // notes are larger, thus Insert at pos. 0
            SchedulablesList.Insert(0, schedulable);
            return 0;
          }
        }
      } while ((left <= right));
      SchedulablesList.Add(schedulable);
      return SchedulablesList.Count - 1;
    }
  }
}