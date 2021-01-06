#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Milan.Simulation.Observers
{
  public interface ISimulationObserver : IDomainEntity
  {
    bool IsEnabled { get; set; }
    IModel Model { get; set; }
    string Name { get; }
    void Flush();
    void Initialize();
    void Reset();
    void Configure(IExperiment experiment);
  }
}