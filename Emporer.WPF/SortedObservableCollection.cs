#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Emporer.WPF
{
  [Serializable]
  public class SortedObservableCollection<T> : ObservableCollection<T>
    where T : class
  {
    private readonly IComparer<T> _comparer;
    private readonly string _propertyNameActivatingReordering;
    private readonly bool _reorderOnPropertyChange;
    private readonly IComparer<T> _selfExcludingComparer;


    public SortedObservableCollection(IComparer<T> comparer)
    {
      _comparer = comparer;
      _selfExcludingComparer = new SelfExcludingComparer<T>(_comparer);
    }

    public SortedObservableCollection(IComparer<T> comparer, string propertyNameActivatingReordering)
    {
      _comparer = comparer;
      _propertyNameActivatingReordering = propertyNameActivatingReordering;
      _reorderOnPropertyChange = !string.IsNullOrEmpty(_propertyNameActivatingReordering);
    }

    public SortedObservableCollection(List<T> initialItems, IComparer<T> comparer, string propertyNameActivatingReordering)
      : this(initialItems.ToArray(), comparer, propertyNameActivatingReordering)
    {
    }

    public SortedObservableCollection(IEnumerable<T> initialItems, IComparer<T> comparer, string propertyNameActivatingReordering)
      : this(comparer, propertyNameActivatingReordering)
    {
      foreach (var item in initialItems)
      {
        Add(item);
      }
    }

    protected override void InsertItem(int index, T item)
    {
      if (_reorderOnPropertyChange)
      {
        var notifyPropertyChanged = (INotifyPropertyChanged) item;
        notifyPropertyChanged.PropertyChanged += Reorder;
      }


      index = Array.BinarySearch(Items.ToArray(), item, _comparer);

      if (index >= 0)
      {
        base.InsertItem(index + 1, item);
        return;
      }

      base.InsertItem(~index, item);
    }

    private void Reorder(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName != _propertyNameActivatingReordering)
      {
        return;
      }
      Reorder((T) sender);
    }

    private void Reorder(T item)
    {
      var items = Items.ToArray();
      var oldIndex = IndexOf(item);
      int newIndex;
      var passedItemDuringTraverse = false;

      for (newIndex = 0; newIndex < items.Count(); newIndex++)
      {
        var existingItem = items[newIndex];

        if (items[newIndex] == item)
        {
          passedItemDuringTraverse = true;
          continue;
        }

        if (IsGreaterThan(existingItem, item))
        {
          break;
        }
      }

      if (newIndex == items.Length || passedItemDuringTraverse)
      {
        //fix special case: end of the list reached
        newIndex--;
      }

      MoveItem(oldIndex, newIndex);
    }

    private bool IsGreaterThan(T first, T second)
    {
      // Makes comparisson more comprehensible.
      return _comparer.Compare(first, second) > 0;
    }

    protected override void RemoveItem(int index)
    {
      var item = this[index];
      var notifyPropertyChanged = item as INotifyPropertyChanged;

      if (_reorderOnPropertyChange && notifyPropertyChanged != null)
      {
        notifyPropertyChanged.PropertyChanged -= Reorder;
      }

      base.RemoveItem(index);
    }


    private class SelfExcludingComparer<U> : IComparer<U>
      where U : class
    {
      private readonly IComparer<U> _WrappedComparer;

      public SelfExcludingComparer(IComparer<U> wrappedComparer)
      {
        _WrappedComparer = wrappedComparer;
      }

      public int Compare(U x, U y)
      {
        return x == y
                 ? 0
                 : _WrappedComparer.Compare(x, y);
      }
    }
  }
}