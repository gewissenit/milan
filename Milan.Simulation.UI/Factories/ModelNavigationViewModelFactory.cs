#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.ComponentModel.Composition;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (IModelNavigationViewModelFactory))]
  internal class ModelNavigationViewModelFactory : IModelNavigationViewModelFactory
  {
    private readonly IEntityNavigationViewModelFactory _entityViewModelFactory;

    [ImportingConstructor]
    public ModelNavigationViewModelFactory([Import] IEntityNavigationViewModelFactory entityViewModelFactory)
    {
      _entityViewModelFactory = entityViewModelFactory;
    }

    public ModelNavigationViewModel CreateNavigationViewModel(IModel model, Func<IEntity, bool> entityFilter = null)
    {
      return new ModelNavigationViewModel(model, _entityViewModelFactory, entityFilter);
    }
  }
}