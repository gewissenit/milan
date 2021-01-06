#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Accessories
{
  public class ConditionalQueue<T> : FiniteQueue<T>
    where T : class, IEntity
  {
    public ConditionalQueue(int maximumCapacity)
      : base(maximumCapacity)
    {
    }

    public virtual T DequeueConditional(IQueueCondition<T> condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException("condition");
      }
      if (IsEmpty)
      {
        throw new InvalidOperationException(
          "Queue is empty! Call CountItemsMeetingCondition(...) before calling DequeueConditional(...) to see if there are any entities available meeting the given condition.");
      }
      foreach (var item in ItemList)
      {
        if (condition.IsMetBy(item))
        {
          UpdateStatistics();
          ItemList.Remove(item);
          return item;
        }
      }
      throw new InvalidOperationException(
        "No entity meeting given Condition found! Call CountItemsMeetingCondition(...) before calling DequeueConditional(...) to see if there are any entities available meeting the given condition.");
    }

    public virtual T PeekConditional(IQueueCondition<T> condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException("condition");
      }
      if (IsEmpty)
      {
        throw new InvalidOperationException(
          "Queue is empty! Call CountItemsMeetingCondition(...) before calling PeekConditional(...) to see if there are any entities available meeting the given condition.");
      }
      foreach (var item in ItemList)
      {
        if (condition.IsMetBy(item))
        {
          return item;
        }
      }
      throw new InvalidOperationException(
        "No entity meeting given Condition found! Call CountItemsMeetingCondition(...) before calling PeekConditional(...) to see if there are any entities available meeting the given condition.");
    }

    public virtual int CountItemsMeetingCondition(IQueueCondition<T> condition)
    {
      if (condition == null)
      {
        throw new ArgumentNullException("condition");
      }
      var count = 0;
      foreach (var item in ItemList)
      {
        if (condition.IsMetBy(item))
        {
          count++;
        }
      }
      return count;
    }
  }
}