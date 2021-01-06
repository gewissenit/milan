#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Milan.JsonStore;
using Milan.Simulation.Observers;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Model : DomainEntity, IModel
  {
    private readonly IDictionary<Type, int> _dynamicEnityCounters = new Dictionary<Type, int>();

    [JsonProperty]
    private readonly ObservableCollection<IEntity> _Entities = new ObservableCollection<IEntity>();
    
    [JsonProperty]
    private readonly IList<ISimulationObserver> _Observers = new List<ISimulationObserver>();
    
    public Model()
    {
      StartDate = DateTime.Now;
      Name = string.Empty;
      ObservableEntities = _Entities;
    }

    //todo: refactor and merge with entity list
    public ObservableCollection<IEntity> ObservableEntities { get; private set; }

    public virtual IEnumerable<IEntity> Entities
    {
      get { return _Entities; }
    }

    [JsonProperty]
    public string Name
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    public IEnumerable<ISimulationObserver> Observers
    {
      get { return _Observers; }
    }

    [JsonProperty]
    public string Description
    {
      get { return Get<string>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public DateTime StartDate
    {
      get { return Get<DateTime>(); }
      set { Set(value); }
    }

    public virtual void Add(IEntity entity)
    {
      if (_Entities.Contains(entity))
      {
        throw new InvalidOperationException(string.Format("The model already contains the given entity: {0}", entity));
      }
      entity.Model = this;
      _Entities.Add(entity);
    }

    public virtual void Add(ISimulationObserver observer)
    {
      if (_Observers.Contains(observer))
      {
        throw new InvalidOperationException(string.Format("The model already contains the given observer: {0}", observer));
      }
      observer.Model = this;
      _Observers.Add(observer);
    }

    public int GetIndexForDynamicEntity(Type entityType)
    {
      if (!_dynamicEnityCounters.ContainsKey(entityType))
      {
        _dynamicEnityCounters.Add(entityType, 0);
      }
      return _dynamicEnityCounters[entityType]++;
    }


    public void Initialize()
    {
      foreach (var entity in Entities)
      {
        entity.Initialize();
      }
    }
    
    public virtual void Remove(IEntity entity)
    {
      if (!_Entities.Contains(entity))
      {
        throw new InvalidOperationException(string.Format("The model does not contain the given entity: {0}", entity));
      }
      _Entities.Remove(entity);
    }

    public virtual void Remove(ISimulationObserver observer)
    {
      if (!_Observers.Contains(observer))
      {
        throw new InvalidOperationException(string.Format("The model does not contain the given observer: {0}", observer));
      }
      _Observers.Remove(observer);
      observer.Model = null;
    }

    public void Reset()
    {
      _dynamicEnityCounters.Clear();
      foreach (var entity in Entities)
      {
        entity.Reset();
      }
      foreach (var observer in Observers)
      {
        observer.Reset();
      }
    }
  }
}