#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using Emporer.Material;
using Emporer.Unit;
using Milan.Simulation.Events;
using Milan.Simulation.MaterialAccounting.ReportDataProviders;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation.MaterialAccounting
{
  [JsonObject(MemberSerialization.OptIn)]
  public class EventMaterialObserver<TEntity, TObservedEvent> : EntityEventObserver<TEntity, TObservedEvent>, IMaterialObserver<TEntity>
    where TEntity : class, IEntity
    where TObservedEvent : class, ISimulationEvent
  {
    private readonly IList<MaterialPosition> _balancePositions = new List<MaterialPosition>();

    protected EventMaterialObserver(string name)
    {
      Name = name;
      Category = string.Empty;
    }

    public IEnumerable<MaterialPosition> BalancePositions
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
    public IMaterial Material
    {
      get { return Get<IMaterial>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IUnit Unit
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
      var value = Unit.ToBaseUnit(Amount);
      CreateBalancePosition(observedEvent, Entity, Material, CurrentExperiment, value, LossRatio, Name, Category);
    }

    protected void CreateBalancePosition(ISimulationEvent e, IEntity station, IMaterial material, IExperiment experiment, double total, double lossRatio, string processName, string category, IProductType productType = null, long productId = -1)
    {
      var loss = total / 100 * lossRatio;
      var totalCosts = material.DisplayUnit.FromBaseUnit(total) * material.Price;
      var lossCosts = material.DisplayUnit.FromBaseUnit(loss) * material.Price;
      var bp = new MaterialPosition
               {
                 Station = station,
                 Currency = material.Currency,
                 Experiment = experiment,
                 Total = total,
                 Loss = loss,
                 TotalCosts = totalCosts,
                 LossCosts = lossCosts,
                 Material = material,
                 Process = processName,
                 Category = category,
                 ProductType = productType,
                 Event = e.Id,
                 Product = productId
               };
      _balancePositions.Add(bp);
      //var ptName = bp.ProductType == null
      //               ? ""
      //               : bp.ProductType.Name;
      //var message = $"{bp.Station.Name};{bp.Process};{ptName};{bp.Category};{bp.Material.Name};{bp.Material.DisplayUnit.BasicUnit.Symbol};{bp.Total};{bp.Loss};{bp.Currency.Symbol};{bp.TotalCosts};{bp.LossCosts};{bp.Product};{bp.Experiment.Id};{bp.Event}";
      //_logger.Append(message);
    }
  }
}