#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  public interface IModelNavigationViewModelFactory
  {
    ModelNavigationViewModel CreateNavigationViewModel(IModel model, Func<IEntity, bool> entityFilter = null);
  }
}