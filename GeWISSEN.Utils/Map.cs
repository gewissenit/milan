using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeWISSEN.Utils
{
  /// <summary>
  ///   Use this class to beautify <c>switch</c> statements.
  ///   It acts like a dictionary with an fallback for nonexisting keys.
  ///   To replace a switch, create it with your switch variable type as key and <see cref="Action" /> as value.
  ///   This class implements <see cref="ICollection" /> so it can be initialized like this:
  ///   <example>
  ///   </example>
  /// </summary>
  public class Map<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
  {
    private readonly TValue _fallback;
    private List<KeyValuePair<TKey, TValue>> _items;

    public Map(TValue fallback = default(TValue))
    {
      _fallback = fallback;
      Clear();
    }

    public TValue this[TKey key]
    {
      get
      {
        var match = _items.SingleOrDefault(i => i.Key.Equals(key));
        return Found(match)
                 ? match.Value
                 : _fallback;
      }
      set
      {
        _items.Add(new KeyValuePair<TKey, TValue>(key, value));
      }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      _items.Add(item);
    }

    public void Clear()
    {
      _items = new List<KeyValuePair<TKey, TValue>>();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return _items.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return _items.Remove(item);
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private static bool Found(KeyValuePair<TKey, TValue> match)
    {
      return !match.Equals(default(KeyValuePair<TKey, TValue>));
    }

    public void Add(TKey key, TValue value)
    {
      _items.Add(new KeyValuePair<TKey, TValue>(key, value));
    }
  }
}