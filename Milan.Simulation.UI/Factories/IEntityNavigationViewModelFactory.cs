#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.UI.Factories
{
  public interface IEntityNavigationViewModelFactory
  {
    INavigatorNode CreateNavigationViewModel(IEntity model);
  }
}