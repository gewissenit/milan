#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Accessories
{
  public class Queue<T>
    where T : class
  {
    #region ctor(s)

    public Queue(ITimeProvider timeProvider)
    {
      _contentCounter = new TimeWeightedValueAccumulator<int>(() => timeProvider.CurrentTime);
      _statisticsActivated = true;
    }

    public Queue()
    {
      _statisticsActivated = false;
    }

    #endregion

    private readonly TimeWeightedValueAccumulator<int> _contentCounter;
    private readonly List<T> _itemList = new List<T>();
    private readonly bool _statisticsActivated;

    private bool StatisticsActivated
    {
      get { return _statisticsActivated; }
    }

    protected List<T> ItemList
    {
      get { return _itemList; }
    }

    private TimeWeightedValueAccumulator<int> ContentCounter
    {
      get { return _contentCounter; }
    }

    public virtual bool IsEmpty
    {
      get { return ItemList.Count == 0; }
    }

    public virtual int Count
    {
      get { return ItemList.Count; }
    }

    public virtual void Enqueue(Queue<T> queue)
    {
      if (queue == null)
      {
        throw new ArgumentNullException("queue");
      }

      UpdateStatistics();
      while (!queue.IsEmpty)
      {
        ItemList.Add(queue.Dequeue());
      }
    }

    public virtual void Enqueue(T item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }

      UpdateStatistics();
      ItemList.Add(item);
    }

    protected virtual void UpdateStatistics()
    {
      if (StatisticsActivated)
      {
        ContentCounter.Update(Count);
      }
    }

    public virtual T Dequeue()
    {
      if (IsEmpty)
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      UpdateStatistics();
      var item = ItemList[0];
      ItemList.RemoveAt(0);
      return item;
    }

    public virtual void Remove(T item)
    {
      ItemList.Remove(item);
    }

    public virtual T Peek()
    {
      if (IsEmpty)
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      return ItemList[0];
    }

    public virtual bool Contains(T item)
    {
      return ItemList.Contains(item);
    }
  }
}