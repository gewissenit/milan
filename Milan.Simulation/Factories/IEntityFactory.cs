#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Factories
{
  public interface IEntityFactory
  {
    string Name { get; }
    bool CanHandle(IEntity entity);
    IEntity Create(IModel model);
    IEntity Duplicate(IEntity entity, IModel destinationModel);
    IEntity CreateSimulationEntity(IEntity entity, IExperiment experiment);
    void ResolveReferences(IEntity entity);
    void Prepare(IEntity entity);
  }
}