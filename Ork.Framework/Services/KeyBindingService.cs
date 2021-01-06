using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Ork.Framework.Services
{
  internal class KeyBindingService : IKeyBindingService
  {
    private static readonly Dictionary<string, Key> _keyNames = new Dictionary<string, Key>
                                                                {
                                                                  {
                                                                    "Alt", Key.LeftAlt
                                                                  },

                                                                  {
                                                                    "Ctrl", Key.LeftCtrl
                                                                  }
                                                                };

    private readonly SortedSet<Key> _pressedKeys = new SortedSet<Key>(Comparer<Key>.Create((k1, k2) => k1.CompareTo(k2)));
    private readonly Dictionary<string, Dictionary<object, Action>> _subscriptions;


    public KeyBindingService()
    {
      // INFO: Key sequences in subscriptions have to be sorted to avoid conflicts (Shift+Alt+E vs Alt+Shift+E).
      // INFO: This is done by parsing the (string) input to the Key enum and using its index to establish an order.
      // INFO: It contains some ugly key names, maybe we use our own map someday.
      _subscriptions = new Dictionary<string, Dictionary<object, Action>>();
    }

    public void SubscribeFor(string keySequence, object subscriber, Action action)
    {
      var sequence = Parse(keySequence);

      if (!_subscriptions.ContainsKey(sequence))
      {
        _subscriptions.Add(sequence, new Dictionary<object, Action>());
      }

      if (_subscriptions[sequence].ContainsKey(subscriber))
      {
        // we do not allow duplicate subscriptions of the same subscriber for the same sequence
        string message = $"The same instance ({subscriber}) is already subscribed to {keySequence}";
        throw new ArgumentException(message);
      }

      _subscriptions[sequence].Add(subscriber, action);
    }

    public void UnsubscribeFor(string keySequence, object subscriber)
    {
    }


    public void ReceiveKeyReleased(string releasedKey, string sender)
    {
    }

    public void ReceiveKeyPressed(string pressedKey, string sender)
    {
      var key = Convert(pressedKey);

      if (_pressedKeys.Contains(key))
      {
        return; // already pressed
      }

      _pressedKeys.Add(key);

      var currentSequence = _pressedKeys.OrderBy(k => k)
                                        .Select(x => x.ToString())
                                        .Aggregate((c, n) => $"{c}|{n}");

      if (!_subscriptions.ContainsKey(currentSequence))
      {
        return; // no subscribers
      }

      foreach (var subscription in _subscriptions[currentSequence])
      {
        subscription.Value(); // subscriber notified
      }
    }

    private string Parse(string keySequence)
    {
      return keySequence.Split('+')
                        .Select(Convert)
                        .OrderBy(k => k)
                        .Select(x => x.ToString())
                        .Aggregate((c, n) => $"{c}|{n}");
    }

    private Key Convert(string input)
    {
      var keyName = input.Trim();
      Key result;
      if (Enum.TryParse(keyName, out result))
      {
        return result;
      }

      if (_keyNames.ContainsKey(keyName))
      {
        return _keyNames[keyName];
      }

      string message = $"{keyName} is not en element of System.Windows.Input.Keys.";
      throw new ArgumentException(message, keyName);
    }
  }
}