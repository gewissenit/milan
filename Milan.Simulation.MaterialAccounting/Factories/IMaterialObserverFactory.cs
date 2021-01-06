#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Factories;

namespace Milan.Simulation.MaterialAccounting.Factories
{
  public interface IMaterialObserverFactory : ISimulationObserverFactory
  {
    string Name { get; }
    bool CanHandle(IEntity entity);
    IMaterialObserver Create();
    IMaterialObserver Duplicate(IMaterialObserver materialObserver);
  }
}