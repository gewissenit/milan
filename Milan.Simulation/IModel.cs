#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Milan.JsonStore;
using Milan.Simulation.Observers;

namespace Milan.Simulation
{
  public interface IModel : IDomainEntity, ISimulationObservable
  {
    string Name { get; set; }
    string Description { get; set; }
    DateTime StartDate { get; set; }
    ObservableCollection<IEntity> ObservableEntities { get; }
    IEnumerable<IEntity> Entities { get; }
    void Add(IEntity entity);
    void Remove(IEntity entity);
    void Initialize();
    void Reset();
    int GetIndexForDynamicEntity(Type entityType);
  }
}