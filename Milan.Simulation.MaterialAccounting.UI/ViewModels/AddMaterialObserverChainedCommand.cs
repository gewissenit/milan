#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Material;
using Emporer.WPF.ViewModels;
using Milan.JsonStore;
using Milan.Simulation.MaterialAccounting.Factories;

namespace Milan.Simulation.MaterialAccounting.UI.ViewModels
{
  public class AddMaterialObserverChainedCommand
  {
    private readonly Action<IMaterialObserver, IMaterial, IEntity//, BalanceDirection
      > _addObserver;
    private readonly IEnumerable<IMaterialObserverFactory> _factories;

    private IEntity _chosenEntity;
    private IMaterial _chosenMaterial;
    private IMaterialObserver _chosenObserver;

    public AddMaterialObserverChainedCommand(IJsonStore store,
                                             IEnumerable<IMaterialObserverFactory> materialObserverFactories,
                                             IModel model,
                                             IEntity selectedEntity,
                                             Action<IMaterialObserver, IMaterial, IEntity//, BalanceDirection
                                               >addObserver)
    {
      _factories = materialObserverFactories;
      _addObserver = addObserver;

      DisplayText = "Add";

      if (model == null)
      {
        // Inactive state
        return;
      }


      IEnumerable<IEntity> entities;
      if (selectedEntity == null)
      {
        entities = model.Entities.OfType<IStationaryElement>();
      }
      else
      {
        entities = new[]
                   {
                     selectedEntity
                   };
      }


      //var balanceSides = new object[]
      //                   {
      //                     BalanceDirection.Input, BalanceDirection.Output
      //                   }.Select(v => new ParameterValue(v, ChooseBalanceSide, _ => new ParameterValue[0]));
      //<-- last parameter, no next param values

      var materials = store.Content.OfType<IMaterial>()
                            .Select(v => new ParameterValue(v,
                                                            ChooseMaterial,
                                                            _ => new ParameterValue[0]));


      var entityValues = entities.Where(entity => _factories.Any(mof => mof.CanHandle(entity)))
                                 .OrderBy(x => x.Name)
                                 .Select(v => new ParameterValue(v,
                                                                 ChooseEntity,
                                                                 entity =>
                                                                 {
                                                                   //todo: create missing observer only on demand when it is selected
                                                                   var observer = _factories.Where(mof => mof.CanHandle((IEntity) entity))
                                                                                            .OrderBy(mof => mof.Name)
                                                                                            .Select(mof => mof.Create());
                                                                   return observer.Select(x => new ParameterValue(x, ChooseObserver, _ => materials));
                                                                 }));
      ValuesForFirstParameter = entityValues;

      if (selectedEntity == null)
      {
        return;
      }

      // special case: entity was selected, skip selecting entities
      _chosenEntity = selectedEntity;
      //todo: create missing observer only on demand when it is selected
      var observerValues = _factories.Where(mof => mof.CanHandle(selectedEntity))
                                     .OrderBy(mof => mof.Name)
                                     .Select(mof => mof.Create())
                                     .Select(x => new ParameterValue(x, ChooseObserver, _ => materials));
      ValuesForFirstParameter = observerValues;
    }

    public string DisplayText { get; private set; }
    public IEnumerable<ParameterValue> ValuesForFirstParameter { get; private set; }

    //private void ChooseBalanceSide(object obj)
    //{
    //  var balanceSide = (BalanceDirection) obj;
    //  _addObserver(_chosenObserver, _chosenMaterial, _chosenEntity, balanceSide);
    //}

    private void ChooseEntity(object obj)
    {
      _chosenEntity = (IEntity) obj;
    }

    private void ChooseMaterial(object obj)
    {
      _chosenMaterial = (IMaterial) obj;
      _addObserver(_chosenObserver, _chosenMaterial, _chosenEntity);
    }

    private void ChooseObserver(object obj)
    {
      _chosenObserver = (IMaterialObserver) obj;
    }
  }
}