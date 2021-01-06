#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Accessories
{
  public class FiniteQueue<T> : Queue<T>
    where T : class
  {
    private int _capacity;

    public FiniteQueue(int maximumCapacity)
    {
      _capacity = maximumCapacity;
    }

    public virtual bool IsFull
    {
      get { return Capacity == Count; }
    }

    public int Capacity
    {
      get { return _capacity; }
      set
      {
        if (value < base.Count)
        {
          throw new ArgumentException(
            "Can not change the queues capacity to a lower value than its current count of item. Please check the count before setting this value.");
        }
        _capacity = value;
      }
    }

    public override void Enqueue(T item)
    {
      if (IsFull)
      {
        throw new InvalidOperationException("Queue is already full! Call IsFull before enqueue new items.");
      }
      base.Enqueue(item);
    }
  }
}