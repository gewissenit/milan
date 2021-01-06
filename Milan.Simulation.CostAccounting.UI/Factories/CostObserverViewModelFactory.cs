#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Unit;
using Milan.JsonStore;
using Milan.Simulation.CostAccounting.Factories;
using Milan.Simulation.CostAccounting.UI.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.CostAccounting.UI.Factories
{
  [Export(typeof (ICostObserverViewModelFactory))]
  internal class CostObserverViewModelFactory : ICostObserverViewModelFactory
  {
    private readonly IEnumerable<ICostObserverFactory> _costObserverFactories;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public CostObserverViewModelFactory([ImportMany] IEnumerable<ICostObserverFactory> costObserverFactories, [Import] IJsonStore store)
    {
      _costObserverFactories = costObserverFactories;
      _store = store;
    }


    public ICostObserverViewModel Create(ICostObserver observer,
                                         IEnumerable<IProductType> availableProductTypes,
                                         IEnumerable<IUnit> currencies,
                                         ObservableCollection<string> categories)
    {
      // currently there's only one viewmodel that can handle all different types of cost observers
      return new CostObserverViewModel(observer,
                                       _store,
                                       _costObserverFactories,
                                       //todo: use factory
                                       availableProductTypes.Select(p => new ProductTypeViewModel(p))
                                                            .ToArray(),
                                       currencies,
                                       categories);
    }
  }
}