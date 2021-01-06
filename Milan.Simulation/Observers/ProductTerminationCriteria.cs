#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Milan.Simulation.Events;
using Newtonsoft.Json;

namespace Milan.Simulation.Observers
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ProductTerminationCriteria : SchedulerObserver, IProductTerminationCriteria
  {
    public ProductTerminationCriteria()
    {
      Name = "Terminate if certain amount of products has left the system.";
    }

    [JsonProperty]
    private readonly IList<IProductTypeAmount> _ProductAmounts = new List<IProductTypeAmount>();

    private readonly IDictionary<IProductType, int> _productsLeft = new Dictionary<IProductType, int>();
    private readonly BehaviorSubject<float> _progressGenerator = new BehaviorSubject<float>(0.0f);

    public IEnumerable<IProductTypeAmount> ProductAmounts
    {
      get { return _ProductAmounts; }
    }

    public IObservable<float> Progress
    {
      get { return _progressGenerator; }
    }

    [JsonProperty]
    public bool HasAndOperator
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    public override void Reset()
    {
      _productsLeft.Clear();
      base.Reset();
    }

    public void Add(IProductTypeAmount productTypeCapacity)
    {
      if (_ProductAmounts.Any(cm => cm.ProductType == productTypeCapacity.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _ProductAmounts.Add(productTypeCapacity);
    }

    public void Remove(IProductTypeAmount productTypeCapacity)
    {
      if (!_ProductAmounts.Contains(productTypeCapacity))
      {
        throw new InvalidOperationException();
      }
      _ProductAmounts.Remove(productTypeCapacity);
    }

    protected override void AdditionalInitialization()
    {
      foreach (var productAmount in ProductAmounts)
      {
        _productsLeft.Add(productAmount.ProductType, 0);
      }
    }

    protected override void OnEventOccurred(ISimulationEvent e)
    {
      if (e is ProductsDestroyedEvent)
      {
        return;
      }

      if (!(e is ThroughputEndEvent))
      {
        return;
      }
      var throughputEnd = (ThroughputEndEvent) e;

      foreach (var product in throughputEnd.Products)
      {
        if (!_productsLeft.ContainsKey(product.ProductType))
        {
          return;
        }

        _productsLeft[product.ProductType]++;

        // notify progress change
        _progressGenerator.OnNext(CalculateProgress());

        //TODO: refactor
        // Trivial problem but the code needs much eyeballing to know whats happening here.
        // The code handles wether all product type amounts have to be fullfilled or just one of them.
        if (HasAndOperator)
        {
          if (_productsLeft.Any(entry => entry.Value < ProductAmounts.Single(pa => pa.ProductType == entry.Key)
                                                                      .Amount))
          {
            return;
          }
        }
        else
        {
          if (_productsLeft[product.ProductType] < ProductAmounts.Single(pa => pa.ProductType == product.ProductType)
                                                                  .Amount)
          {
            return;
          }
        }
      }

      // criterion is met -> stop experiment
      var simulationEndEvent = new SimulationEndEvent(e.Sender, CurrentExperiment);
      Scheduler.ScheduleAfter(simulationEndEvent, Scheduler.CurrentSchedulable);
    }

    private float CalculateProgress()
    {
      return HasAndOperator
               ? GetCummulativeProgress()
               : GetMaxProgress();
    }

    private float GetCummulativeProgress()
    {
      var cumulativeProducts = _productsLeft.Join(ProductAmounts, l => l.Key, r => r.ProductType, (l, r) => Cap(l.Value, r.Amount))
                                             .Sum();

      var cummulativeAmounts = ProductAmounts.Sum(pta => pta.Amount);
      var result = (float) cumulativeProducts / cummulativeAmounts * 100;
      return result;
    }

    private float GetMaxProgress()
    {
      return _productsLeft.Join(ProductAmounts, l => l.Key, r => r.ProductType, (l, r) => (float) Cap(l.Value, r.Amount) / r.Amount * 100)
                           .Max();
    }

    private int Cap(int value, int cap)
    {
      // if value is less than cap return value, else cap
      return value < cap
               ? value
               : cap;
    }
  }
}