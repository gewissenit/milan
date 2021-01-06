#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Unit;
using Emporer.WPF.ViewModels;
using Milan.Simulation.CostAccounting.Factories;

namespace Milan.Simulation.CostAccounting.UI.ViewModels
{
  public class AddCostObserverChainedCommand
  {
    private readonly Action<ICostObserver, IUnit, IEntity> _addObserver;
    private readonly IEnumerable<ICostObserverFactory> _factories;
    private IUnit _chosenCurrency;
    private IEntity _chosenEntity;
    private ICostObserver _chosenObserver;

    public AddCostObserverChainedCommand(IEnumerable<ICostObserverFactory> costObserverFactories,
                                         IEnumerable<IUnit> currencies,
                                         IModel model,
                                         IEntity selectedEntity,
                                         Action<ICostObserver, IUnit, IEntity> addObserver)
    {
      var currencyParameters = currencies.Select(x => new ParameterValue(x, ChooseCurrency, _ => new ParameterValue[0]));
      _factories = costObserverFactories;
      IModel model1 = model;
      _addObserver = addObserver;

      DisplayText = "Add";

      if (model1 == null)
      {
        // Inactive state
        return;
      }

      IEnumerable<IEntity> entities;
      if (selectedEntity == null)
      {
        entities = model1.Entities.OfType<IStationaryElement>();
      }
      else
      {
        entities = new[]
                   {
                     selectedEntity
                   };
      }

      var entityValues = entities.Where(entity => _factories.Any(cof => cof.CanHandle(entity)))
                                 .OrderBy(x => x.Name)
                                 .Select(v => new ParameterValue(v,
                                                                 ChooseEntity,
                                                                 entity =>
                                                                 {
                                                                   //todo: create missing observer only on demand when it is selected
                                                                   var factory = _factories.Where(cof => cof.CanHandle((IEntity) entity))
                                                                                            .OrderBy(cof => cof.Name)
                                                                                            .Select(cof => cof.Create());
                                                                   return
                                                                     factory.Select(
                                                                                    x =>
                                                                                    new ParameterValue(x, ChooseObserver, _ => currencyParameters));
                                                                 }));

      ValuesForFirstParameter = entityValues;

      if (selectedEntity == null)
      {
        return;
      }

      // special case: entity was selected, skip selecting entities
      _chosenEntity = selectedEntity;
      //todo: create missing observer only on demand when it is selected
      var observerValues = _factories.Where(cof => cof.CanHandle(selectedEntity))
                                      .OrderBy(cof => cof.Name)
                                      .Select(cof => cof.Create())
                                      .Select(x => new ParameterValue(x, ChooseObserver, _ => currencyParameters));
      ValuesForFirstParameter = observerValues;
    }

    public string DisplayText { get; private set; }
    public IEnumerable<ParameterValue> ValuesForFirstParameter { get; private set; }

    private void ChooseCurrency(object obj)
    {
      _chosenCurrency = (IUnit) obj;
      _addObserver(_chosenObserver, _chosenCurrency, _chosenEntity);
    }

    private void ChooseEntity(object obj)
    {
      _chosenEntity = (IEntity) obj;
    }

    private void ChooseObserver(object obj)
    {
      _chosenObserver = (ICostObserver) obj;
    }
  }
}