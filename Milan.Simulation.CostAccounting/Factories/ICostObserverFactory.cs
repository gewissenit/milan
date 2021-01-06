#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Factories;

namespace Milan.Simulation.CostAccounting.Factories
{
  public interface ICostObserverFactory : ISimulationObserverFactory
  {
    string Name { get; }
    bool CanHandle(IEntity entity);
    ICostObserver Create();
    ICostObserver Duplicate(ICostObserver materialObserver);
  }
}