#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  /// <summary>
  ///   <see cref="InterpolatedTimeTable" /> comprising the center element of the list to use it as new bound if
  ///   interpolation delivers adverse values. Takes less steps (half) than <see cref="InterpolatedTimeTable" /> or
  ///   <see cref="BinaryTimeTable" />, but calculation is more complex.
  /// </summary>
  public class AdvancedInterpolatedTimeTable : TimeTable
  {
    #region Overrides of TimeTable

    /// <summary>
    ///   dependent on the sort method
    ///   A own strategy to sort the <paramref name="schedulable" /> objects is needful because the option with a generic
    ///   Comparison of a List.Sort-method have a implementation performs an unstable sort; that is, if two elements are equal,
    ///   their order might not be preserved. In contrast, a stable sort preserves the order of elements that are equal. In a
    ///   simulation application it's necessary to have a control over the order might of two equal elements.
    /// </summary>
    /// <param name="schedulable"></param>
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

      var leftIndex = 0;
      var rightIndex = SchedulablesList.Count - 1;
      ISimulationEvent center;
      ISimulationEvent relative;

      do
      {
        if (SchedulablesList[leftIndex + 1].ScheduledTime > refTime)
        {
          SchedulablesList.Insert(leftIndex + 1, schedulable);
          return leftIndex + 1;
        }
        if (SchedulablesList[rightIndex - 1].ScheduledTime <= refTime)
        {
          SchedulablesList.Insert(rightIndex, schedulable);
          return rightIndex;
        }

        var centerIndex = (rightIndex + leftIndex) / 2;
        center = SchedulablesList[centerIndex];

        var percentage = (refTime - leftTime) / (rightTime - leftTime);
        var relativeIndex = (int) ((rightIndex - leftIndex) * percentage + leftIndex);
        relative = SchedulablesList[relativeIndex];

        var centerTime = center.ScheduledTime;
        var relativeTime = relative.ScheduledTime;

        var l = leftIndex;
        var r = rightIndex;

        if (centerTime <= refTime)
        {
          leftIndex = centerIndex;
          leftTime = centerTime;
        }
        else
        {
          rightIndex = centerIndex;
          rightTime = centerTime;
        }

        if (relativeTime <= refTime)
        {
          if (relativeIndex > leftIndex)
          {
            leftIndex = relativeIndex;
            leftTime = relativeTime;
          }
        }
        else
        {
          if (relativeIndex < rightIndex)
          {
            rightIndex = relativeIndex;
            rightTime = relativeTime;
          }
        }
        if (l == leftIndex &&
            r == rightIndex)
        {
          leftIndex++;
          leftTime = SchedulablesList[leftIndex].ScheduledTime;
          if (leftIndex < rightIndex)
          {
            rightIndex--;
            rightTime = SchedulablesList[rightIndex].ScheduledTime;
          }
        }
      } while (leftIndex < rightIndex);
      throw new InvalidOperationException("Something went wrong!");
    }

    #endregion
  }
}