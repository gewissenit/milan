#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  [Export(typeof (IEntityNavigationViewModelFactory))]
  internal class EntityNavigationViewModelFactory : IEntityNavigationViewModelFactory
  {
    public INavigatorNode CreateNavigationViewModel(IEntity model)
    {
      return new EntityNavigationViewModel(model);
    }
  }
}