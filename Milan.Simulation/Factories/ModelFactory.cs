#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Milan.JsonStore;
using Milan.Simulation.Observers;
using Milan.VisualModeling.Persistence;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IModelFactory))]
  internal class ModelFactory : IModelFactory
  {
    private readonly IEnumerable<IEntityFactory> _entityFactories;
    private readonly IEnumerable<ISimulationObserverFactory> _simulationObserverFactories;
    private readonly IJsonStore _store;
    private readonly IEnumerable<ITerminationCriteriaFactory> _terminationCriteriaFactories;

    [ImportingConstructor]
    public ModelFactory([Import] IJsonStore store,
                        [ImportMany] IEnumerable<IEntityFactory> entityFactories,
                        [ImportMany] IEnumerable<ISimulationObserverFactory> simulationObserverFactories,
                        [ImportMany] IEnumerable<ITerminationCriteriaFactory> terminationCriteriaFactories)
    {
      _store = store;
      _entityFactories = entityFactories;
      _simulationObserverFactories = simulationObserverFactories;
      _terminationCriteriaFactories = terminationCriteriaFactories;
    }

    public IModel Create()
    {
      var model = new Model();
      var visualModel = new ModelConfiguration(model);
      _store.Add(model);
      _store.Add(visualModel);
      return model;
    }

    public IModel Duplicate(IModel model)
    {
      var clone = Clone(model);
      clone.Name = GetUniqueName(model.Name);

      foreach (var entity in model.Entities)
      {
        var entityClone = _entityFactories.Single(ef => ef.CanHandle(entity))
                                           .Duplicate(entity, clone);
        //hack: entity id should be the same to find references.
        entityClone.Id = entity.Id;
      }

      foreach (var observer in model.Observers.OfType<ITerminationCriteria>())
      {
        clone.Add(_terminationCriteriaFactories.Single(sof => sof.CanHandle(observer))
                                                .Duplicate(observer));
      }

      foreach (var entity in clone.Entities)
      {
        _entityFactories.Single(ef => ef.CanHandle(entity))
                         .ResolveReferences(entity);
      }

      foreach (var observer in clone.Observers)
      {
        _simulationObserverFactories.Single(sof => sof.CanHandle(observer))
                                     .Prepare(observer);
      }

      _store.Add(clone);

      var originalModelConfiguration = _store.Content.OfType<ModelConfiguration>()
                                              .SingleOrDefault(x => x.Model == model);
      
      if (originalModelConfiguration != null)
      {
        var clonedVisualModelConfiguration = new ModelConfiguration(clone);

        _store.Add(clonedVisualModelConfiguration);

        foreach (var entity in clone.Entities)
        {

          // clone visual cfg
          var originalVisualConfig = GetMatchingVisualConfiguration(entity, originalModelConfiguration);
          clonedVisualModelConfiguration.Visuals.Add(new VisualConfiguration(entity)
                                                     {
                                                       X = originalVisualConfig.X,
                                                       Y = originalVisualConfig.Y,
                                                       Height = originalVisualConfig.Height,
                                                       Width = originalVisualConfig.Width
                                                     });
        }
      }
      return clone;
    }

    public void Delete(IModel model)
    {
      _store.Remove(model);
    }

    public IModel CreateSimulationModel(IModel model, IEnumerable<IStatisticalObserverFactory> statisticalObserverFactories, IExperiment experiment)
    {
      var clone = Clone(model);
      clone.Id = model.Id;
      clone.Name = model.Name;
      experiment.Model = clone;

      foreach (var entity in model.Entities)
      {
        _entityFactories.Single(ef => ef.CanHandle(entity))
                         .CreateSimulationEntity(entity, experiment);
      }

      foreach (var observer in model.Observers)
      {
        clone.Add(_simulationObserverFactories.Single(sof => sof.CanHandle(observer))
                                               .CreateSimulationObserver(observer, experiment));
      }

      Prepare(clone);

      foreach (var factory in statisticalObserverFactories)
      {
        clone.Add(factory.Create(experiment));
      }

      return clone;
    }

    private IModel Clone(IModel model)
    {
      var modelClone = new Model
                       {
                         Description = model.Description,
                         StartDate = model.StartDate
                       };

      return modelClone;
    }

    private void Prepare(IModel model)
    {
      foreach (var entity in model.Entities)
      {
        _entityFactories.Single(ef => ef.CanHandle(entity))
                         .Prepare(entity);
      }

      foreach (var observer in model.Observers)
      {
        _simulationObserverFactories.Single(sof => sof.CanHandle(observer))
                                     .Prepare(observer);
      }
    }

    private VisualConfiguration GetMatchingVisualConfiguration(IEntity targetEntity, ModelConfiguration originalModelConfiguration)
    {
      return originalModelConfiguration.Visuals.SingleOrDefault(x =>
                                                                {
                                                                  var entity = x.Model as IEntity;
                                                                  if (entity == null)
                                                                  {
                                                                    return false;
                                                                  }
                                                                  return entity.Id == targetEntity.Id;
                                                                });
    }

    private string GetUniqueName(string original)
    {
      var names = _store.Content.OfType<IModel>()
                         .Select(e => e.Name);
      return JsonStore.Utils.GetUniqueName(original, names);
    }
  }
}