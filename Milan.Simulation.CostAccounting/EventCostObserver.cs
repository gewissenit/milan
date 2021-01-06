#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Emporer.Unit;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.CostAccounting
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EventCostObserver<TEntity, TObservedEvent> : EntityEventObserver<TEntity, TObservedEvent>, ICostObserver<TEntity>, IEventCostObserver<TEntity, TObservedEvent>
    where TEntity : class, IEntity
    where TObservedEvent : class, ISimulationEvent
  {
    private readonly IList<Position> _balancePositions = new List<Position>();

    protected EventCostObserver(string name)
    {
      Name = name;
      Category = string.Empty;
    }

    public IEnumerable<Position> BalancePositions
    {
      get { return _balancePositions; }
    }

    [JsonProperty]
    public double Amount
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double LossRatio
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public string Category
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IUnit Currency
    {
      get { return Get<IUnit>(); }
      set { Set(value); }
    }

    public override void Reset()
    {
      _balancePositions.Clear();
      base.Reset();
    }

    public override string ToString()
    {
      return $"{Name}";
    }

    protected override void OnEntityEventOccurred(TObservedEvent observedEvent)
    {
      CreateBalancePosition(observedEvent, Entity, Currency, CurrentExperiment, Amount, LossRatio, Name, Category);
    }

    protected void CreateBalancePosition(ISimulationEvent e, IEntity station, IUnit currency, IExperiment experiment, double total, double lossRatio, string processName, string category, IProductType productType = null, long productId = -1)
    {
      var loss = total / 100 * lossRatio;
      var bp = new Position
               {
                 Station = station,
                 Currency = currency,
                 Experiment = experiment,
                 Total = total,
                 Loss = loss,
                 Process = processName,
                 Category = category,
                 Event = e.Id,
                 ProductType = productType,
                 Product = productId
               };
      _balancePositions.Add(bp);
      var ptName = bp.ProductType == null
                     ? ""
                     : bp.ProductType.Name;

      var message = $"{bp.Station.Name};{bp.Process};{ptName};{bp.Category};{bp.Currency.Symbol};{bp.Total};{bp.Loss};{bp.Product};{bp.Experiment.Id},{bp.Event}";
      _logger.Append(message);
    }
  }
}