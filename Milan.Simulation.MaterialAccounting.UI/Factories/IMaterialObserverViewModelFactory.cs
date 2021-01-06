#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;

using Milan.Simulation.MaterialAccounting.UI.ViewModels;

namespace Milan.Simulation.MaterialAccounting.UI.Factories
{
  public interface IMaterialObserverViewModelFactory
  {
    IMaterialObserverViewModel Create(IMaterialObserver observer,
                                      IEnumerable<IProductType> availableProductTypes,
                                      ObservableCollection<string> categories);
  }
}