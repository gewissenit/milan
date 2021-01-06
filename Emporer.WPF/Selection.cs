using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Threading;

namespace Emporer.WPF
{
  [Export(typeof (ISelection))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class Selection : ISelection
  {
    private readonly Subject<SelectionChange> _selectionChanges = new Subject<SelectionChange>();
    private readonly IDictionary<object, IDictionary<Type, IDisposable>> _subscriptions = new Dictionary<object, IDictionary<Type, IDisposable>>();

    public Selection()
    {
      // subscribe myself to get current selection
      _selectionChanges.Subscribe(selection =>
      {
        Current = selection.Selection;
      });
    }

    public void Select<TSelection>(TSelection selection, object sender)
    {
      if ((object)selection == Current)
      {
        return;
      }
      var change = new SelectionChange(sender, selection);
      var sw = Stopwatch.StartNew();
      Console.WriteLine("Changing selection.");
      _selectionChanges.OnNext(change);
      Console.WriteLine("Selection changed. Took {0} ms.", sw.ElapsedMilliseconds);
      sw.Stop();
    }

    public void Subscribe<TSelection>(object subscriber, Action<TSelection> onSelection)
    {
      var scheduler = new EventLoopScheduler(ts => new Thread(ts));
      var subscription = _selectionChanges.Where(c=> typeof(TSelection).IsAssignableFrom(c.SelectionType) || c.SelectionType == null)
                                          .Where(change => change.Sender != subscriber)
                                          .Select(change => change.Selection)
                                          .Cast<TSelection>()
                                          .Subscribe(onSelection);

      Subscribe<TSelection>(subscriber, subscription);
    }

    public void Unsubscribe<TSelection>(object subscriber)
    {
      if (!_subscriptions.ContainsKey(subscriber))
      {
        return;
      }

      var selectionType = typeof (TSelection);

      foreach (var entry in _subscriptions[subscriber].Where(entry => selectionType.IsAssignableFrom(entry.Key))
                                                      .ToArray())
      {
        var subscription = _subscriptions[subscriber][entry.Key];
        _subscriptions[subscriber].Remove(entry.Key);
        subscription.Dispose();
      }
      if (!_subscriptions[subscriber].Any())
      {
        _subscriptions.Remove(subscriber);
      }
    }

    public object Current { get; private set; }

    private void Subscribe<TSelection>(object subscriber, IDisposable subscription)
    {
      if (!_subscriptions.ContainsKey(subscriber))
      {
        _subscriptions.Add(subscriber, new Dictionary<Type, IDisposable>());
      }
      var selectionType = typeof (TSelection);
      if (!_subscriptions[subscriber].ContainsKey(selectionType))
      {
        _subscriptions[subscriber].Add(selectionType, subscription);
      }
      else
      {
        _subscriptions[subscriber][selectionType] = subscription;
      }
    }

    private class SelectionChange
    {
      public SelectionChange(object sender, object selection)
      {
        Sender = sender;
        Selection = selection;
        if (selection==null)
        {
          return;
        }
        SelectionType = selection.GetType();
      }

      public object Sender { get; set; }
      public object Selection { get; set; }
      public Type SelectionType { get; private set; }
    }
  }
}