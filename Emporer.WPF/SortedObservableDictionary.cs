#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Emporer.WPF
{
  [Serializable]
  public class SortedObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
  {
    [NonSerialized]
    private readonly SerializationInfo _siInfo;

    private IComparer<DictionaryEntry> _comparer;

    public SortedObservableDictionary(IComparer<DictionaryEntry> comparer)
    {
      _comparer = comparer;
    }

    public SortedObservableDictionary(IComparer<DictionaryEntry> comparer, IDictionary<TKey, TValue> dictionary)
      : base(dictionary)
    {
      _comparer = comparer;
    }

    public SortedObservableDictionary(IComparer<DictionaryEntry> comparer, IEqualityComparer<TKey> equalityComparer)
      : base(equalityComparer)
    {
      _comparer = comparer;
    }

    public SortedObservableDictionary(IComparer<DictionaryEntry> comparer,
                                      IDictionary<TKey, TValue> dictionary,
                                      IEqualityComparer<TKey> equalityComparer)
      : base(dictionary, equalityComparer)
    {
      _comparer = comparer;
    }

    protected SortedObservableDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      _siInfo = info;
    }

    protected override bool AddEntry(TKey key, TValue value)
    {
      var entry = new DictionaryEntry(key, value);
      var index = GetInsertionIndexForEntry(entry);
      _keyedEntryCollection.Insert(index, entry);
      return true;
    }

    protected virtual int GetInsertionIndexForEntry(DictionaryEntry newEntry)
    {
      return BinaryFindInsertionIndex(0, Count - 1, newEntry);
    }

    protected override bool SetEntry(TKey key, TValue value)
    {
      var keyExists = _keyedEntryCollection.Contains(key);

      // if identical key/value pair already exists, nothing to do
      if (keyExists && value.Equals((TValue) _keyedEntryCollection[key].Value))
      {
        return false;
      }

      // otherwise, remove the existing entry
      if (keyExists)
      {
        _keyedEntryCollection.Remove(key);
      }

      // add the new entry
      var entry = new DictionaryEntry(key, value);
      var index = GetInsertionIndexForEntry(entry);
      _keyedEntryCollection.Insert(index, entry);

      return true;
    }

    private int BinaryFindInsertionIndex(int first, int last, DictionaryEntry entry)
    {
      if (last < first)
      {
        return first;
      }
      else
      {
        var mid = first + ((last - first) / 2);
        var result = _comparer.Compare(_keyedEntryCollection[mid], entry);
        if (result == 0)
        {
          return mid;
        }
        else if (result < 0)
        {
          return BinaryFindInsertionIndex(mid + 1, last, entry);
        }
        else
        {
          return BinaryFindInsertionIndex(first, mid - 1, entry);
        }
      }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException("info");
      }

      if (!_comparer.GetType()
                    .IsSerializable)
      {
        throw new NotSupportedException("The supplied Comparer is not serializable.");
      }

      base.GetObjectData(info, context);
      info.AddValue("_comparer", _comparer);
    }

    public override void OnDeserialization(object sender)
    {
      if (_siInfo != null)
      {
        _comparer = (IComparer<DictionaryEntry>) _siInfo.GetValue("_comparer", typeof (IComparer<DictionaryEntry>));
      }
      base.OnDeserialization(sender);
    }
  }
}