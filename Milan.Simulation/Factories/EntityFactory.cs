#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Factories
{
  public abstract class EntityFactory : IEntityFactory
  {
    private readonly IEnumerable<IAdditionalEntityDuplicationAction> _duplicationActions;

    protected EntityFactory(string name, IEnumerable<IAdditionalEntityDuplicationAction> duplicationActions)
    {
      _duplicationActions = duplicationActions;
      Name = name;
    }

    public abstract bool CanHandle(IEntity entity);

    public string Name { get; private set; }

    public IEntity Create(IModel model)
    {
      var item = Create();
      model.Add(item);
      item.Model = model;
      return item;
    }

    public IEntity Duplicate(IEntity entity, IModel destinationModel)
    {
      var clone = Copy(entity);

      clone.Name = GetUniqueName(destinationModel, entity);
      clone.Description = entity.Description;

      destinationModel.Add(clone);

      foreach (var additionalEntityDuplicationAction in _duplicationActions)
      {
        additionalEntityDuplicationAction.DuplicateEntity(destinationModel, entity, clone);
      }
      return clone;
    }

    public IEntity CreateSimulationEntity(IEntity entity, IExperiment experiment)
    {
      var clone = CreateSimulationEntity(entity);
      clone.Id = entity.Id;
      clone.Name = entity.Name;
      clone.Description = entity.Description;
      clone.CurrentExperiment = experiment;
      experiment.Model.Add(clone);
      return clone;
    }

    public virtual void Prepare(IEntity entity)
    {
    }

    public virtual void ResolveReferences(IEntity entity)
    {
    }

    protected abstract IEntity Copy(IEntity entity);
    protected abstract IEntity CreateSimulationEntity(IEntity entity);
    protected abstract IEntity Create();

    private static string GetUniqueName(IModel model, IEntity master)
    {
      var names = model.Entities.Select(e => e.Name);
      return JsonStore.Utils.GetUniqueName(master.Name, names);
    }
  }
}