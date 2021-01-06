#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Milan.Simulation;
using Milan.Simulation.Events;
using Milan.Simulation.Observers;
using Milan.Simulation.Statistics;

namespace EcoFactory.Components.Observers
{
  public sealed class StorageObserver : EntityTypeObserver<IStorage>
  {
    private const string Empty = "Empty";
    private const string Available = "Available";
    private const string Full = "Full";
    private readonly ICollection<FillLevelStatistic> _fillLevels = new List<FillLevelStatistic>();
    private readonly IDictionary<IEntity, KeyValuePair<string, double>> _previousStates = new Dictionary<IEntity, KeyValuePair<string, double>>();
    private readonly ICollection<NumberOfProductsAtPlace> _remainingProducts = new List<NumberOfProductsAtPlace>();
    private readonly IDictionary<IEntity, ICollection<ProcessStatistic>> _states = new Dictionary<IEntity, ICollection<ProcessStatistic>>();
    private readonly IDictionary<IEntity, IDictionary<Product, double>> _storageContents = new Dictionary<IEntity, IDictionary<Product, double>>();
    private readonly IList<UnfinishedProcess> _unfinishedProcesses = new List<UnfinishedProcess>();

    //HACK: we use dicts instead of flat lists internally (for speed?)
    public IEnumerable<FillLevelStatistic> FillLevels
    {
      get { return _fillLevels; }
    }

    public IEnumerable<NumberOfProductsAtPlace> RemainingProducts
    {
      get { return _remainingProducts; }
    }

    public IEnumerable<ProcessStatistic> States
    {
      get
      {
        //flattening dict of dicts
        return _states.Values.SelectMany(dict => dict);
      }
    }

    public IEnumerable<UnfinishedProcess> UnfinishedProcesses
    {
      get { return _unfinishedProcesses; }
    }

    public override void Reset()
    {
      _fillLevels.Clear();
      _previousStates.Clear();
      _remainingProducts.Clear();
      _states.Clear();
      _storageContents.Clear();
      _unfinishedProcesses.Clear();
      base.Reset();
    }

    public override string ToString()
    {
      return $"Observer: Storage processes (for {typeof(IStorage).Name})";
    }

    protected override void AdditionalInitialization()
    {
      foreach (var storage in Model.Entities.OfType<IStorage>())
      {
        PrepareStates(storage);
        RetrieveInitialStates(storage);
      }
    }
    
    protected override void OnEntityTypeEventOccurred(ISimulationEvent e)
    {
      if (!(e is IProductsRelatedEvent) ||
          !(e.Sender is IStorage))
      {
        return;
      }

      var productRelatedEvent = (IProductsRelatedEvent) e;
      var entity = (IStorage) e.Sender;
      var product = productRelatedEvent.Products.Single(); //assuming Products.Count() is always 1

      var previousState = _previousStates[entity].Key;
      var newState = GetStorageState(entity);

      if (newState != previousState)
      {
        UpdateStateStatistic(entity, previousState, product.ProductType.Name);

        _previousStates[entity] = new KeyValuePair<string, double>(newState, CurrentExperiment.CurrentTime);
      }

      if (productRelatedEvent is ProductTransmitEvent)
      {
        UpdateFillLevelStatistics(productRelatedEvent);
        //clear entry in dictionary so that the product can be stored again if neccessary
        var storage = (IEntity) e.Sender;
        if (_storageContents.ContainsKey(storage)) // HACK: because of a SETTLING TIME a product can be unknown (not in the dict) here.
        {
          _storageContents[storage].Remove(product);
        }
      }
      else if (productRelatedEvent is ProductReceiveEvent)
      {
        // remember product to measure retention times (tTransmit-tReceive)
        if (!_storageContents.ContainsKey(entity))
        {
          _storageContents.Add(entity, new Dictionary<Product, double>());
        }
        _storageContents[entity].Add(product, e.ScheduledTime);
      }
    }

    protected override void OnSimulationEnd(ISimulationEvent endEvent)
    {
      var storages = Model.Entities.OfType<IStorage>()
                          .ToArray();

      foreach (var storage in storages)
      {
        foreach (var productTypeAmount in storage.ProductTypeCounts)
        {
          _remainingProducts.Add(new NumberOfProductsAtPlace(storage.Name, productTypeAmount.ProductType.Name, productTypeAmount.Amount));
          var previousState = _previousStates[storage];
          _unfinishedProcesses.Add(new UnfinishedProcess(storage.Name,
                                                          previousState.Key,
                                                          new[]
                                                          {
                                                            productTypeAmount.ProductType.Name
                                                          },
                                                          0d,
                                                          CurrentExperiment.CurrentTime - previousState.Value));
        }
      }
    }

    private ProcessStatistic AddNewProcessEntry(string entityName,
                                                string processName,
                                                string productTypeName,
                                                ICollection<ProcessStatistic> statisticsCollection)
    {
      var processStatistic = new ProcessStatistic(entityName, processName, productTypeName, () => CurrentExperiment.CurrentTime);
      statisticsCollection.Add(processStatistic);
      return processStatistic;
    }

    private void FindAndUpdateProcessStatistic(ICollection<ProcessStatistic> statisticsCollection,
                                               string entityName,
                                               string processName,
                                               string productTypeName,
                                               TimeSpan duration)
    {
      //TODO: exists also in StationaryElementProcessObserver -> inherit
      var matchingProcess =
        statisticsCollection.SingleOrDefault(p => p.Entity == entityName && p.Process == processName && p.ProductType == productTypeName) ??
        AddNewProcessEntry(entityName, processName, productTypeName, statisticsCollection);

      UpdateProcessEntry(matchingProcess, duration);
    }

    private static int GetCapacity(IStorage storage, IProductType productType)
    {
      if (!storage.HasLimitedCapacity)
      {
        return int.MaxValue;
      }

      if (!storage.HasCapacityPerProductType)
      {
        return storage.Capacity;
      }

      var pta = storage.ProductTypeCapacities.SingleOrDefault(ptc => ptc.ProductType == productType);
      if (pta != null)
      {
        return pta.Amount;
      }

      throw new InvalidOperationException("This should not occur!");
    }

    private FillLevelStatistic GetMatchingFillLevel(IStorage storage, IProductType productType)
    {
      var level = FillLevels.SingleOrDefault(fl => fl.Storage == storage.Name && fl.ProductType == productType.Name);

      if (level != null)
      {
        return level;
      }

      var capacity = GetCapacity(storage, productType);
      level = new FillLevelStatistic(storage.Name, productType.Name, capacity, () => CurrentExperiment.CurrentTime);
      _fillLevels.Add(level);
      return level;
    }

    private static string GetStorageState(IStorage storage)
    {
      if (storage.Count == 0)
      {
        return Empty;
      }
      if (storage.HasLimitedCapacity &&
          storage.Count == storage.Capacity)
      {
        return Full;
      }
      return Available;
    }

    private void PrepareStates(IStorage storage)
    {
      _states.Add(storage, new List<ProcessStatistic>());
    }

    private void RetrieveInitialStates(IStorage storage)
    {
      var stateName = GetStorageState(storage);
      var currentTime = CurrentExperiment.CurrentTime;
      var startProcess = new KeyValuePair<string, double>(stateName, currentTime);
      _previousStates.Add(storage, startProcess);
    }

    private void UpdateFillLevelStatistics(IProductsRelatedEvent productRelatedEvent)
    {
      foreach (var productType in productRelatedEvent.Products.Select(p => p.ProductType))
      {
        var storage = (IStorage) productRelatedEvent.Sender;
        var fillLevelStatistic = GetMatchingFillLevel(storage, productType);
        var fillLevel = storage.GetCount(productType);
        fillLevelStatistic.Values.Update(fillLevel);
      }
    }


    private void UpdateProcessEntry(ProcessStatistic matchingProcess, TimeSpan duration)
    {
      //TODO: exists also in StationaryElementProcessObserver -> inherit
      matchingProcess.Durations.Update(duration);
    }

    private void UpdateStateStatistic(IEntity storage, string state, string productTypeName)
    {
      //TODO: exists also in StationaryElementProcessObserver -> inherit
      var simTimeSpan = CurrentExperiment.CurrentTime - _previousStates[storage].Value;
      var duration = simTimeSpan.ToRealTimeSpan();
      FindAndUpdateProcessStatistic(_states[storage], storage.Name, state, productTypeName, duration);
    }
  }
}