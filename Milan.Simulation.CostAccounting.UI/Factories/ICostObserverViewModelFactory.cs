#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Emporer.Unit;
using Milan.Simulation.CostAccounting.UI.ViewModels;

namespace Milan.Simulation.CostAccounting.UI.Factories
{
  public interface ICostObserverViewModelFactory
  {
    ICostObserverViewModel Create(ICostObserver observer,
                                  IEnumerable<IProductType> availableProductTypes,
                                  IEnumerable<IUnit> currencies,
                                  ObservableCollection<string> categories);
  }
}