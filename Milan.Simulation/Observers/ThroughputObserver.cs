#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation.Events;
using Milan.Simulation.Reporting;

namespace Milan.Simulation.Observers
{
  public class ThroughputObserver : EntityTypeObserver<IStationaryElement>
  {
    private readonly IDictionary<Product, double> _arrivalTimes = new Dictionary<Product, double>();
    private readonly ICollection<long> _destroyedProducts = new List<long>();
    private readonly IDictionary<Product, double> _enteredTimes = new Dictionary<Product, double>();
    private readonly ICollection<long> _remainingProducts = new List<long>();
    private readonly IList<Position> _statisticPositions = new List<Position>();
    private readonly ICollection<long> _successfulProducts = new List<long>();
    private double _smallestStartDate;

    public IEnumerable<long> DestroyedProducts => _destroyedProducts;
    public IEnumerable<long> RemainingProducts => _remainingProducts;
    public IEnumerable<long> SuccessfulProducts => _successfulProducts;
    public IEnumerable<Position> StatisticPositions => _statisticPositions;

    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      var throughputStart = e as ThroughputStartEvent;
      if (throughputStart != null)
      {
        foreach (var product in throughputStart.Products)
        {
          _enteredTimes.Add(product, throughputStart.ScheduledTime);
        }
        return;
      }
      var arrivalStart = e as ProductReceiveEvent;
      if (arrivalStart != null)
      {
        foreach (var product in arrivalStart.Products)
        {
          _arrivalTimes.Add(product, arrivalStart.ScheduledTime);
        }
        return;
      }
      var arrivalEnd = e as ProductTransmitEvent;
      if (arrivalEnd != null)
      {
        foreach (var product in arrivalEnd.Products)
        {
          if (!_arrivalTimes.ContainsKey(product))
          {
            //this can only occurre when products are transformed successfully -> take earliest entering product time
            if (!product.IntegratedProducts.Any())
            {
              throw new InvalidOperationException("This should not occurre!");
            }
            var integratedProducts = _arrivalTimes.Where(at => product.IntegratedProducts.Contains(at.Key))
                                                  .ToArray();
            if (integratedProducts.Any())
            {
              _smallestStartDate = integratedProducts.Min(at => at.Value);
            }
            CreatePosition(e, product, _smallestStartDate, Constants.StationThroughput);
            foreach (var integratedProduct in integratedProducts.Select(ip => ip.Key))
            {
              _arrivalTimes.Remove(integratedProduct);
              _enteredTimes.Remove(integratedProduct);
            }
            _enteredTimes.Add(product, _smallestStartDate);
          }
          else
          {
            CreatePosition(e, product, _arrivalTimes[product], Constants.StationThroughput);
            _arrivalTimes.Remove(product);
          }
        }
        return;
      }
      var destroyed = e as ProductsDestroyedEvent;
      if (destroyed != null)
      {
        foreach (var product in destroyed.Products)
        {
          _destroyedProducts.Add(product.Id);
          if (!_enteredTimes.ContainsKey(product))
          {
            //this can only occurre when product is transformed but not finished
            if (!product.IntegratedProducts.Any())
            {
              throw new InvalidOperationException("This should not occurre!");
            }
            var integratedProducts = _enteredTimes.Where(at => product.IntegratedProducts.Contains(at.Key))
                                                  .ToArray();

            if (integratedProducts.Any())
            {
              //this can only occurre when several products (output >1) are transformed
              _smallestStartDate = integratedProducts.Min(at => at.Value);
            }
            CreatePosition(e, product, _smallestStartDate, Constants.DestroyedThroughput);
            foreach (var integratedProduct in integratedProducts.Select(ip=>ip.Key))
            {
              _arrivalTimes.Remove(integratedProduct);
              _enteredTimes.Remove(integratedProduct);
            }
          }
          else
          {
            CreatePosition(e, product, _enteredTimes[product], Constants.DestroyedThroughput);
            _arrivalTimes.Remove(product);
            _enteredTimes.Remove(product);
          }
        }
        return;
      }
      var throughputEnd = e as ThroughputEndEvent;
      if (throughputEnd != null)
      {
        foreach (var product in throughputEnd.Products)
        {
          _successfulProducts.Add(product.Id);
          CreatePosition(e, product, _enteredTimes[product], Constants.SystemThroughput);
          _enteredTimes.Remove(product);
        }
      }
    }

    private void CreatePosition(ISimulationEvent e, Product product, double startDate, string process, IEntity entity = null)
    {
      var start = Model.StartDate.AddMilliseconds(startDate);
      var end = Model.StartDate.AddMilliseconds(e.ScheduledTime);
      var station = entity ?? (IEntity) e.Sender;
      _statisticPositions.Add(new Position
                              {
                                Station = station,
                                Event = e.Id,
                                ProductType = product.ProductType,
                                Process = process,
                                Experiment = CurrentExperiment,
                                Product = product.Id,
                                StartDate = start,
                                EndDate = end
                              });
    }

    public override void Reset()
    {
      _arrivalTimes.Clear();
      _enteredTimes.Clear();
      _statisticPositions.Clear();
      _destroyedProducts.Clear();
      _remainingProducts.Clear();
      _successfulProducts.Clear();
      base.Reset();
    }

    public override string ToString()
    {
      return "Observer: Account product end type";
    }

    protected override void OnSimulationEnd(ISimulationEvent e)
    {
      foreach (var productEntered in _enteredTimes)
      {
        _remainingProducts.Add(productEntered.Key.Id);
        CreatePosition(e, productEntered.Key, productEntered.Value, Constants.RemainedThroughput, productEntered.Key.CurrentLocation);
      }
    }
  }
}