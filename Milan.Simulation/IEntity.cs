#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Milan.Simulation
{
  public interface IEntity : IDomainEntity
  {
    string Name { get; set; }
    string Description { get; set; }
    IModel Model { get; set; }
    IExperiment CurrentExperiment { get; set; }
    void Initialize();
    void Reset();
  }
}