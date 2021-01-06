#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Accessories
{
  public class FiniteQueueCluster<TEntity, TCategory>
    where TEntity : class
    where TCategory : class
  {
    private readonly List<CategorizedEntity<TEntity, TCategory>> _itemList = new List<CategorizedEntity<TEntity, TCategory>>();
    private readonly Dictionary<TCategory, Queue<TEntity>> _queueDictionary = new Dictionary<TCategory, Queue<TEntity>>();

    public FiniteQueueCluster()
    {
      _queueDictionary = new Dictionary<TCategory, Queue<TEntity>>();
    }

    protected IDictionary<TCategory, Queue<TEntity>> QueueDictionary
    {
      get { return _queueDictionary; }
    }

    public IEnumerable<TEntity> StoredItems
    {
      get { return _itemList.Select(ce => ce.Entity); }
    }

    public virtual IEnumerable<TCategory> Categories
    {
      get { return new MutableQueueDictionaryKeys<TCategory, Queue<TEntity>>(_queueDictionary); }
    }

    public int? OverallCapacity { get; set; }

    public void SetCapacity(int capacity, TCategory category)
    {
      if (OverallCapacity.HasValue)
      {
        if (OverallCapacity.Value < capacity)
        {
          throw new InvalidOperationException("You cannot set a capacity smaller than the overall capacity!"); //NON-NLS-1
        }
      }
      if (!QueueDictionary.ContainsKey(category))
      {
        QueueDictionary.Add(category, new FiniteQueue<TEntity>(capacity));
      }
      else
      {
        if (QueueDictionary[category].Count > capacity)
        {
          throw new InvalidOperationException("You cannot set a capacity smaller than the amount of already existing items of this category!");
          //NON-NLS-1
        }

        var queueWithNewCapacity = new FiniteQueue<TEntity>(capacity);

        while (!QueueDictionary[category].IsEmpty)
        {
          queueWithNewCapacity.Enqueue(QueueDictionary[category].Dequeue());
        }

        QueueDictionary[category] = queueWithNewCapacity;
      }
    }

    public virtual bool IsFull(TCategory category)
    {
      if (_itemList.Count == OverallCapacity)
      {
        return true;
      }

      if (!QueueDictionary.ContainsKey(category))
      {
        return false;
      }
      if (QueueDictionary[category] is FiniteQueue<TEntity>)
      {
        var finiteQueue = (FiniteQueue<TEntity>) QueueDictionary[category];
        return finiteQueue.IsFull;
      }
      else
      {
        return false;
      }
    }

    public virtual bool IsEmpty()
    {
      if (_itemList.Count == 0)
      {
        return true;
      }
      return false;
    }

    public virtual bool Empty(TCategory category)
    {
      if (QueueDictionary.ContainsKey(category))
      {
        return QueueDictionary[category].IsEmpty;
      }
      return true;
    }

    public virtual int GetCount(TCategory category)
    {
      return QueueDictionary[category].Count;
    }

    public virtual int GetCount()
    {
      var count = 0;
      foreach (var category in QueueDictionary.Keys)
      {
        count += QueueDictionary[category].Count;
      }
      return count;
    }

    public virtual void Enqueue(TEntity item, TCategory category)
    {
      if (IsFull(category))
      {
        throw new InvalidOperationException("Ask whether the queue is full before you add!");
      }
      Queue<TEntity> queue;
      if (QueueDictionary.ContainsKey(category))
      {
        queue = QueueDictionary[category];
      }
      else
      {
        queue = new Queue<TEntity>();
        QueueDictionary.Add(category, queue);
      }
      queue.Enqueue(item);
      _itemList.Add(new CategorizedEntity<TEntity, TCategory>(item, category));
      RaiseItemEnqueued(item, category);
    }

    public virtual void Remove(TEntity item)
    {
      var categorizedEntity = GetCategoryForEntity(item);
      _itemList.Remove(categorizedEntity);
      QueueDictionary[categorizedEntity.Category].Remove(item);
    }

    public virtual TEntity Dequeue()
    {
      if (IsEmpty())
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      return Dequeue(_itemList[0].Category);
    }

    public virtual TEntity Dequeue(TCategory category)
    {
      if (Empty(category))
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      var wasFullBefore = IsFull(category);
      var entity = QueueDictionary[category].Dequeue();
      foreach (var categorizedEntity in _itemList)
      {
        if (categorizedEntity.Entity != entity ||
            !Equals(categorizedEntity.Category, category))
        {
          continue;
        }
        _itemList.Remove(categorizedEntity);
        break;
      }

      RaiseItemDequeued(entity, category);

      //raise event if queue was full
      if (wasFullBefore)
      {
        RaiseNotFullAnymore(entity, category);
      }

      return entity;
    }

    public virtual TEntity Peek(TCategory category)
    {
      if (Empty(category))
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      return QueueDictionary[category].Peek();
    }

    public virtual TEntity Peek()
    {
      if (IsEmpty())
      {
        throw new InvalidOperationException("Queue is empty!");
      }
      return Peek(_itemList[0].Category);
    }

    public virtual void Clear()
    {
      QueueDictionary.Clear();
      _itemList.Clear();
    }

    public void Clear(TCategory category)
    {
      while (!QueueDictionary[category].IsEmpty)
      {
        Dequeue(category);
      }
    }

    public bool TryGetNextEntityOfDifferentCategory(TEntity prevEntity, out TEntity nextEntity)
    {
      var prevCategorizedEntity = GetCategoryForEntity(prevEntity);
      for (var i = _itemList.IndexOf(prevCategorizedEntity); i < _itemList.Count; i++)
      {
        if (prevCategorizedEntity.Category != _itemList[i].Category)
        {
          nextEntity = _itemList[i].Entity;
          return true;
        }
      }
      nextEntity = null;
      return false;
    }

    private CategorizedEntity<TEntity, TCategory> GetCategoryForEntity(TEntity prevEntity)
    {
      foreach (var categorizedEntity in _itemList)
      {
        if (categorizedEntity.Entity == prevEntity)
        {
          return categorizedEntity;
        }
      }
      throw new InvalidOperationException(string.Format("Queue does not contains {0}", prevEntity));
    }

    public event EventHandler<FiniteQueueEventArgs> ItemEnqueued;
    public event EventHandler<FiniteQueueEventArgs> ItemDequeued;
    public event EventHandler<FiniteQueueEventArgs> NotFullAnymore;

    private void RaiseItemDequeued(TEntity entity, TCategory category)
    {
      if (ItemDequeued != null)
      {
        ItemDequeued(this, new FiniteQueueEventArgs(category));
      }
    }

    private void RaiseItemEnqueued(TEntity entity, TCategory category)
    {
      if (ItemEnqueued != null)
      {
        ItemEnqueued(this, new FiniteQueueEventArgs(category));
      }
    }

    private void RaiseNotFullAnymore(TEntity entity, TCategory category)
    {
      if (NotFullAnymore != null)
      {
        NotFullAnymore(this, new FiniteQueueEventArgs(category));
      }
    }

    private class MutableQueueDictionaryKeys<TKey, TValue> : ICollection<TKey>
      where TValue : Queue<TEntity>, new()
    {
      private readonly IDictionary<TKey, TValue> _ControlledDictionary;

      public MutableQueueDictionaryKeys(IDictionary<TKey, TValue> dictionary)
      {
        _ControlledDictionary = dictionary;
      }

      public void Add(TKey item)
      {
        _ControlledDictionary.Add(item, new TValue());
      }


      public void Clear()
      {
        _ControlledDictionary.Clear();
      }

      public bool Contains(TKey item)
      {
        return _ControlledDictionary.ContainsKey(item);
      }

      public void CopyTo(TKey[] array, int arrayIndex)
      {
        _ControlledDictionary.Keys.CopyTo(array, arrayIndex);
      }

      public int Count
      {
        get { return _ControlledDictionary.Keys.Count; }
      }

      public bool IsReadOnly
      {
        get { return false; }
      }

      /// <exception cref="InvalidOperationException">
      ///   This item can not be removed because it still acts as an identification for
      ///   a collection that is not empty. Remove all items of that collection before removing the key!
      /// </exception>
      /// <exception cref="ArgumentException">Item could not be removed because it was not in the collection.</exception>
      public bool Remove(TKey item)
      {
        if (!_ControlledDictionary.ContainsKey(item))
        {
          return false;
          //throw new ArgumentException("Item could not be removed because it was not in the collection.");
        }

        if (_ControlledDictionary[item].Count > 0)
        {
          return false;
          //throw new InvalidOperationException("This item can not be removed because it still acts as an identification for a collection that is not empty. Remove all items of that collection before removing the key!");
        }

        _ControlledDictionary.Remove(item);
        return true;
      }

      public IEnumerator<TKey> GetEnumerator()
      {
        return _ControlledDictionary.Keys.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _ControlledDictionary.GetEnumerator();
      }
    }
  }
}